using System;
using System.Runtime.CompilerServices;
using PrototypePackages.JobUtils.Template;
using PrototypePackages.MathematicsUtils.Index;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense.Greedy;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
	public static class RectsGeneration
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static NativeParallelHashSet<T> ListToSet<T>(NativeQueue<T> list)
			where T : unmanaged, IEquatable<T>
		{
			var set = new NativeParallelHashSet<T>(list.Count, Allocator.Temp);
			while (list.TryDequeue(out T value))
			{
				set.Add(value);
			}
			return set;
		}

		public static void ContainerListsToSets<T>(Container6Dir<NativeQueue<T>> lists, out Container6Dir<NativeParallelHashSet<T>> sets)
			where T : unmanaged, IEquatable<T>
		{
			sets = new()
			{
				plus_x = ListToSet(lists.plus_x),
				mnus_x = ListToSet(lists.mnus_x),
				plus_y = ListToSet(lists.plus_y),
				mnus_y = ListToSet(lists.mnus_y),
				plus_z = ListToSet(lists.plus_z),
				mnus_z = ListToSet(lists.mnus_z)
			};
		}

		public static void Greedy_QuadSetToLineSet<TIndexWalker>(Index3D coordinate, NativeParallelHashSet<int2> quad_set, NativeParallelHashSet<int3> line_set)
			where TIndexWalker : unmanaged, IIndexWalker
		{
			var walker = new TIndexWalker();
			foreach (var (block_id, start_pos) in quad_set)
			{
				coordinate.To3D(start_pos, out var x, out var y, out var z);

				var (beside_x, beside_y, beside_z) = (x, y, z);
				walker.Walk(ref beside_x, ref beside_y, ref beside_z, -1);
				coordinate.To1D(beside_x, beside_y, beside_z, out var beside_pos);

				if (quad_set.Contains(new(block_id, beside_pos))) { continue; }

				(beside_x, beside_y, beside_z) = (x, y, z);
				var end_pos = start_pos;
				while (true)
				{
					walker.Walk(ref beside_x, ref beside_y, ref beside_z, 1);
					coordinate.To1D(beside_x, beside_y, beside_z, out beside_pos);
					if (!quad_set.Contains(new(block_id, beside_pos))) { break; }
					end_pos = beside_pos;
				}
				line_set.Add(new(block_id, start_pos, end_pos));
			}
		}

		public static void Greedy_LineSetToRectSet<TIndexWalker>(Index3D coordinate, NativeParallelHashSet<int3> line_set, NativeParallelHashSet<int3> rect_set)
			where TIndexWalker : unmanaged, IIndexWalker
		{
			var walker = new TIndexWalker();
			foreach (var (block_id, start_pos, end_pos) in line_set)
			{
				coordinate.To3D(start_pos, out var start_x, out var start_y, out var start_z);
				coordinate.To3D(end_pos, out var end_x, out var end_y, out var end_z);

				var (beside_start_x, beside_start_y, beside_start_z) = (start_x, start_y, start_z);
				walker.Walk(ref beside_start_x, ref beside_start_y, ref beside_start_z, -1);
				coordinate.To1D(beside_start_x, beside_start_y, beside_start_z, out var beside_start_pos);

				var (beside_end_x, beside_end_y, beside_end_z) = (end_x, end_y, end_z);
				walker.Walk(ref beside_end_x, ref beside_end_y, ref beside_end_z, -1);
				coordinate.To1D(beside_end_x, beside_end_y, beside_end_z, out var beside_end_pos);

				if (line_set.Contains(new(block_id, beside_start_pos, beside_end_pos))) { return; }

				(beside_start_x, beside_start_y, beside_start_z) = (start_x, start_y, start_z);
				(beside_end_x, beside_end_y, beside_end_z) = (end_x, end_y, end_z);
				var rect_end_pos = end_pos;
				while (true)
				{
					walker.Walk(ref beside_start_x, ref beside_start_y, ref beside_start_z, 1);
					coordinate.To1D(beside_start_x, beside_start_y, beside_start_z, out beside_start_pos);
					walker.Walk(ref beside_end_x, ref beside_end_y, ref beside_end_z, 1);
					coordinate.To1D(beside_end_x, beside_end_y, beside_end_z, out beside_end_pos);
					if (!line_set.Contains(new(block_id, beside_start_pos, beside_end_pos))) { break; }
					rect_end_pos = beside_end_pos;
				}
				rect_set.Add(new(block_id, start_pos, rect_end_pos));
			}
		}

		[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
		public struct GenRectSetsJob : IJob, IPlan
		{
			[NoAlias] Index3D coordinate;
			[NoAlias] [ReadOnly] NativeArray<ushort> volume_chunk;
			[NoAlias] [ReadOnly] NativeArray<ushort> volume_chunk_neighbor_x;
			[NoAlias] [ReadOnly] NativeArray<ushort> volume_chunk_neighbor_y;
			[NoAlias] [ReadOnly] NativeArray<ushort> volume_chunk_neighbor_z;
			[NoAlias] [ReadOnly] NativeArray<bool> inside_chunk;
			[NoAlias] [ReadOnly] NativeArray<bool> inside_chunk_neighbor_x;
			[NoAlias] [ReadOnly] NativeArray<bool> inside_chunk_neighbor_y;
			[NoAlias] [ReadOnly] NativeArray<bool> inside_chunk_neighbor_z;
			[NoAlias] [WriteOnly] Container6Dir<NativeParallelHashSet<int3>> rect_sets;

			public void Execute()
			{
				int3 chunk_size = coordinate.size;

				//1. Gen quads
				var quad_queues = new Container6Dir<NativeQueue<int2>>
				{
					plus_x = new(Allocator.Temp),
					mnus_x = new(Allocator.Temp),
					plus_y = new(Allocator.Temp),
					mnus_y = new(Allocator.Temp),
					plus_z = new(Allocator.Temp),
					mnus_z = new(Allocator.Temp)
				};

				var list = new NativeList<int>();

				int i, i_x_f, i_y_f, i_z_f;
				//1.1. Gen inside quads
				for (int z_0 = 0; z_0 < chunk_size.z - 1; z_0++)
				{
					for (int y_0 = 0; y_0 < chunk_size.y - 1; y_0++)
					{
						for (int x_0 = 0; x_0 < chunk_size.x - 1; x_0++)
						{
							coordinate.To1D(x_0, y_0, z_0, out i);
							coordinate.To1D(x_0 + 1, y_0, z_0, out i_x_f);
							coordinate.To1D(x_0, y_0 + 1, z_0, out i_y_f);
							coordinate.To1D(x_0, y_0, z_0 + 1, out i_z_f);
							var cur_inside = inside_chunk[i];
							if (cur_inside)
							{
								var cur_id = volume_chunk[i];
								if (!inside_chunk[i_x_f]) { quad_queues.plus_x.Enqueue(new(cur_id, i)); }
								if (!inside_chunk[i_y_f]) { quad_queues.plus_y.Enqueue(new(cur_id, i)); }
								if (!inside_chunk[i_z_f]) { quad_queues.plus_z.Enqueue(new(cur_id, i)); }
							}
							else
							{
								if (inside_chunk[i_x_f]) { quad_queues.mnus_x.Enqueue(new(volume_chunk[i_x_f], i)); }
								if (inside_chunk[i_y_f]) { quad_queues.mnus_y.Enqueue(new(volume_chunk[i_y_f], i)); }
								if (inside_chunk[i_z_f]) { quad_queues.mnus_z.Enqueue(new(volume_chunk[i_z_f], i)); }
							}
						}
					}
				}

				//1.2 Gen chunk edge quads
				int x_xf = chunk_size.x - 1;
				for (int z_xf = 0; z_xf < chunk_size.z; z_xf++)
				{
					for (int y_xf = 0; y_xf < chunk_size.y; y_xf++)
					{
						coordinate.To1D(x_xf, y_xf, z_xf, out i);
						coordinate.To1D(0, y_xf, z_xf, out i_x_f);
						var cur_inside = inside_chunk[i];
						if (cur_inside)
						{
							if (!inside_chunk_neighbor_x[i_x_f]) { quad_queues.plus_x.Enqueue(new(volume_chunk[i], i)); }
						}
						else
						{
							if (inside_chunk_neighbor_x[i_x_f]) { quad_queues.mnus_x.Enqueue(new(volume_chunk_neighbor_x[i_x_f], i)); }
						}
					}
				}

				int y_yf = chunk_size.y - 1;
				for (int z_yf = 0; z_yf < chunk_size.z; z_yf++)
				{
					for (int x_yf = 0; x_yf < chunk_size.x; x_yf++)
					{
						coordinate.To1D(x_xf, y_yf, z_yf, out i);
						coordinate.To1D(x_xf, 0, z_yf, out i_y_f);
						var cur_inside = inside_chunk[i];
						if (cur_inside)
						{
							if (!inside_chunk_neighbor_y[i_y_f]) { quad_queues.plus_y.Enqueue(new(volume_chunk[i], i)); }
						}
						else
						{
							if (inside_chunk_neighbor_y[i_y_f]) { quad_queues.mnus_y.Enqueue(new(volume_chunk_neighbor_y[i_y_f], i)); }
						}
					}
				}

				int z_zf = chunk_size.z - 1;
				for (int y_zf = 0; y_zf < chunk_size.y; y_zf++)
				{
					for (int x_zf = 0; x_zf < chunk_size.x; x_zf++)
					{
						coordinate.To1D(x_zf, y_zf, z_zf, out i);
						coordinate.To1D(x_zf, y_zf, 0, out i_z_f);
						var cur_inside = inside_chunk[i];
						if (cur_inside)
						{
							if (!inside_chunk_neighbor_z[i_z_f]) { quad_queues.plus_z.Enqueue(new(volume_chunk[i], i)); }
						}
						else
						{
							if (inside_chunk_neighbor_z[i_z_f]) { quad_queues.mnus_z.Enqueue(new(volume_chunk_neighbor_z[i_z_f], i)); }
						}
					}
				}

				//2. Greedy Mesh

				//2.1 Greedy Line
				ContainerListsToSets(quad_queues, out var quad_sets);
				var line_sets = new Container6Dir<NativeParallelHashSet<int3>>
				{
					plus_x = new(1, Allocator.Temp),
					mnus_x = new(1, Allocator.Temp),
					plus_y = new(1, Allocator.Temp),
					mnus_y = new(1, Allocator.Temp),
					plus_z = new(1, Allocator.Temp),
					mnus_z = new(1, Allocator.Temp)
				};
				Greedy_QuadSetToLineSet<X_Quad_Walker>(coordinate, quad_sets.plus_x, line_sets.plus_x);
				Greedy_QuadSetToLineSet<X_Quad_Walker>(coordinate, quad_sets.mnus_x, line_sets.mnus_x);
				Greedy_QuadSetToLineSet<Y_Quad_Walker>(coordinate, quad_sets.plus_y, line_sets.plus_y);
				Greedy_QuadSetToLineSet<Y_Quad_Walker>(coordinate, quad_sets.mnus_y, line_sets.mnus_y);
				Greedy_QuadSetToLineSet<Z_Quad_Walker>(coordinate, quad_sets.plus_z, line_sets.plus_z);
				Greedy_QuadSetToLineSet<Z_Quad_Walker>(coordinate, quad_sets.mnus_z, line_sets.mnus_z);

				//2.2 Greedy Rect
				Greedy_LineSetToRectSet<X_Line_Walker>(coordinate, line_sets.plus_x, rect_sets.plus_x);
				Greedy_LineSetToRectSet<X_Line_Walker>(coordinate, line_sets.mnus_x, rect_sets.mnus_x);
				Greedy_LineSetToRectSet<Y_Line_Walker>(coordinate, line_sets.plus_y, rect_sets.plus_y);
				Greedy_LineSetToRectSet<Y_Line_Walker>(coordinate, line_sets.mnus_y, rect_sets.mnus_y);
				Greedy_LineSetToRectSet<Z_Line_Walker>(coordinate, line_sets.plus_z, rect_sets.plus_z);
				Greedy_LineSetToRectSet<Z_Line_Walker>(coordinate, line_sets.mnus_z, rect_sets.mnus_z);

			}

			public GenRectSetsJob(int3 chunk_size,
				NativeArray<ushort> volume_chunk,
				NativeArray<ushort> volume_chunk_neighbor_x,
				NativeArray<ushort> volume_chunk_neighbor_y,
				NativeArray<ushort> volume_chunk_neighbor_z,
				NativeArray<bool> inside_chunk,
				NativeArray<bool> inside_chunk_neighbor_x,
				NativeArray<bool> inside_chunk_neighbor_y,
				NativeArray<bool> inside_chunk_neighbor_z,
				out Container6Dir<NativeParallelHashSet<int3>> rect_sets)
			{
				rect_sets = new()
				{
					plus_x = new(1, Allocator.TempJob),
					mnus_x = new(1, Allocator.TempJob),
					plus_y = new(1, Allocator.TempJob),
					mnus_y = new(1, Allocator.TempJob),
					plus_z = new(1, Allocator.TempJob),
					mnus_z = new(1, Allocator.TempJob)
				};
				coordinate = new(chunk_size);
				this.volume_chunk = volume_chunk;
				this.volume_chunk_neighbor_x = volume_chunk_neighbor_x;
				this.volume_chunk_neighbor_y = volume_chunk_neighbor_y;
				this.volume_chunk_neighbor_z = volume_chunk_neighbor_z;
				this.inside_chunk = inside_chunk;
				this.inside_chunk_neighbor_x = inside_chunk_neighbor_x;
				this.inside_chunk_neighbor_y = inside_chunk_neighbor_y;
				this.inside_chunk_neighbor_z = inside_chunk_neighbor_z;
				this.rect_sets = rect_sets;
			}
		}
	}
}