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
		public static NativeParallelHashSet<T> ListToSet<T>(NativeStream list)
			where T : unmanaged, IEquatable<T>
		{
			var reader = list.AsReader();
			reader.BeginForEachIndex(0);
			var count = reader.RemainingItemCount;
			var set = new NativeParallelHashSet<T>(count, Allocator.Temp);
			for (int i = 0; i < count; i++)
			{
				set.Add(reader.Read<T>());
			}
			reader.EndForEachIndex();
			return set;
		}

		public static void ContainerListsToSets<T>(Container6Dir<NativeStream> lists, out Container6Dir<NativeParallelHashSet<T>> sets)
			where T : unmanaged, IEquatable<T>
		{
			sets = new()
			{
				plus_x = ListToSet<T>(lists.plus_x),
				mnus_x = ListToSet<T>(lists.mnus_x),
				plus_y = ListToSet<T>(lists.plus_y),
				mnus_y = ListToSet<T>(lists.mnus_y),
				plus_z = ListToSet<T>(lists.plus_z),
				mnus_z = ListToSet<T>(lists.mnus_z)
			};
		}

		public static void Greedy_QuadSetToLineSet<TIndexWalker>(Index3D coordinate, NativeParallelHashSet<int2> quad_set, NativeParallelHashSet<int3> line_set)
			where TIndexWalker : unmanaged, IIndexWalker
		{
			var walker = new TIndexWalker();
			var quad_set_reader = quad_set.AsReadOnly();
			foreach (var (block_id, start_pos) in quad_set_reader)
			{
				coordinate.To3D(start_pos, out var x, out var y, out var z);

				var (beside_x, beside_y, beside_z) = (x, y, z);
				walker.Walk(ref beside_x, ref beside_y, ref beside_z, -1);
				coordinate.To1D(beside_x, beside_y, beside_z, out var beside_pos);

				if (!coordinate.OutOfRange(beside_x, beside_y, beside_z) &&
					quad_set_reader.Contains(new(block_id, beside_pos)))
				{
					continue;
				}

				(beside_x, beside_y, beside_z) = (x, y, z);
				var end_pos = start_pos;
				while (true)
				{
					walker.Walk(ref beside_x, ref beside_y, ref beside_z, 1);
					if (coordinate.OutOfRange(beside_x, beside_y, beside_z)) { break; }
					coordinate.To1D(beside_x, beside_y, beside_z, out beside_pos);
					if (!quad_set_reader.Contains(new(block_id, beside_pos))) { break; }
					end_pos = beside_pos;
				}
				line_set.Add(new(block_id, start_pos, end_pos));
			}
		}

		public static void Greedy_LineSetToRectSet<TIndexWalker>(Index3D coordinate, NativeParallelHashSet<int3> line_set, NativeParallelHashSet<int3> rect_set)
			where TIndexWalker : unmanaged, IIndexWalker
		{
			var walker = new TIndexWalker();
			var line_set_reader = line_set.AsReadOnly();
			foreach (var (block_id, start_pos, end_pos) in line_set_reader)
			{
				coordinate.To3D(start_pos, out var start_x, out var start_y, out var start_z);
				coordinate.To3D(end_pos, out var end_x, out var end_y, out var end_z);

				var (beside_start_x, beside_start_y, beside_start_z) = (start_x, start_y, start_z);
				walker.Walk(ref beside_start_x, ref beside_start_y, ref beside_start_z, -1);
				coordinate.To1D(beside_start_x, beside_start_y, beside_start_z, out var beside_start_pos);

				var (beside_end_x, beside_end_y, beside_end_z) = (end_x, end_y, end_z);
				walker.Walk(ref beside_end_x, ref beside_end_y, ref beside_end_z, -1);
				coordinate.To1D(beside_end_x, beside_end_y, beside_end_z, out var beside_end_pos);

				if (!coordinate.OutOfRange(beside_start_x, beside_start_y, beside_start_z)
					&& line_set_reader.Contains(new(block_id, beside_start_pos, beside_end_pos)))
				{
					continue;
				}

				(beside_start_x, beside_start_y, beside_start_z) = (start_x, start_y, start_z);
				(beside_end_x, beside_end_y, beside_end_z) = (end_x, end_y, end_z);
				var rect_end_pos = end_pos;
				while (true)
				{
					walker.Walk(ref beside_start_x, ref beside_start_y, ref beside_start_z, 1);
					if (coordinate.OutOfRange(beside_start_x, beside_start_y, beside_start_z)) { break; }
					coordinate.To1D(beside_start_x, beside_start_y, beside_start_z, out beside_start_pos);
					walker.Walk(ref beside_end_x, ref beside_end_y, ref beside_end_z, 1);
					coordinate.To1D(beside_end_x, beside_end_y, beside_end_z, out beside_end_pos);
					if (!line_set_reader.Contains(new(block_id, beside_start_pos, beside_end_pos))) { break; }
					rect_end_pos = beside_end_pos;
				}
				rect_set.Add(new(block_id, start_pos, rect_end_pos));

			}


		}

		[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
		public struct GenRectSetsJob : IJob, IPlan
		{
			[NoAlias] [ReadOnly] int inside_block_group;
			[NoAlias] [ReadOnly] NativeArray<int> block_group_by_id;

			[NoAlias] Index3D coordinate;
			[NoAlias] [ReadOnly] NativeArray<ushort> block_chunk;
			[NoAlias] [ReadOnly] NativeArray<ushort> block_chunk_forward_x;
			[NoAlias] [ReadOnly] NativeArray<ushort> block_chunk_forward_y;
			[NoAlias] [ReadOnly] NativeArray<ushort> block_chunk_forward_z;
			[NoAlias] [WriteOnly] Container6Dir<NativeParallelHashSet<int3>> rect_sets;

			public void Execute()
			{
				int3 chunk_size = coordinate.size;

				var x_f_block_buffer = default(ushort);
				var y_f_block_buffer = new NativeArray<ushort>(coordinate.size.x, Allocator.Temp);
				var z_f_block_buffer = new NativeArray<ushort>(coordinate.size_xy, Allocator.Temp);
				//1. Gen quads
				var quad_streams = new Container6Dir<NativeStream>()
				{
					plus_x = new(1, Allocator.Temp),
					mnus_x = new(1, Allocator.Temp),
					plus_y = new(1, Allocator.Temp),
					mnus_y = new(1, Allocator.Temp),
					plus_z = new(1, Allocator.Temp),
					mnus_z = new(1, Allocator.Temp)
				};
				var quad_streams_w = new Container6Dir<NativeStream.Writer>()
				{
					plus_x = quad_streams.plus_x.AsWriter(),
					mnus_x = quad_streams.mnus_x.AsWriter(),
					plus_y = quad_streams.plus_y.AsWriter(),
					mnus_y = quad_streams.mnus_y.AsWriter(),
					plus_z = quad_streams.plus_z.AsWriter(),
					mnus_z = quad_streams.mnus_z.AsWriter()
				};

				quad_streams_w.plus_x.BeginForEachIndex(0);
				quad_streams_w.mnus_x.BeginForEachIndex(0);
				quad_streams_w.plus_y.BeginForEachIndex(0);
				quad_streams_w.mnus_y.BeginForEachIndex(0);
				quad_streams_w.plus_z.BeginForEachIndex(0);
				quad_streams_w.mnus_z.BeginForEachIndex(0);

				for (int z = chunk_size.z - 1; 0 <= z; z--)
				{
					var part_z = z * coordinate.size_xy;
					for (int y = chunk_size.y - 1; 0 <= y; y--)
					{
						var part_y = y * chunk_size.x;
						for (int x = chunk_size.x - 1; 0 <= x; x--)
						{
							var part_xy = x + part_y;
							int i = part_xy + part_z;
							var x_f_block = x != chunk_size.x - 1 ? x_f_block_buffer : block_chunk_forward_x[i - x];
							var y_f_block = y != chunk_size.y - 1 ? y_f_block_buffer[x] : block_chunk_forward_y[i - part_y];
							var z_f_block = z != chunk_size.z - 1 ? z_f_block_buffer[part_xy] : block_chunk_forward_z[part_xy];
							var cur_block = block_chunk[i];
							var cur_group = block_group_by_id[cur_block];
							if (cur_group == inside_block_group)
							{
								if (block_group_by_id[x_f_block] != inside_block_group) { quad_streams_w.plus_x.Write(new int2(cur_block, i)); }
								if (block_group_by_id[y_f_block] != inside_block_group) { quad_streams_w.plus_y.Write(new int2(cur_block, i)); }
								if (block_group_by_id[z_f_block] != inside_block_group) { quad_streams_w.plus_z.Write(new int2(cur_block, i)); }
							}
							else
							{
								if (block_group_by_id[x_f_block] == inside_block_group) { quad_streams_w.mnus_x.Write(new int2(x_f_block, i)); }
								if (block_group_by_id[y_f_block] == inside_block_group) { quad_streams_w.mnus_y.Write(new int2(y_f_block, i)); }
								if (block_group_by_id[z_f_block] == inside_block_group) { quad_streams_w.mnus_z.Write(new int2(z_f_block, i)); }
							}

							z_f_block_buffer[part_xy] = cur_block;
							y_f_block_buffer[x] = cur_block;
							x_f_block_buffer = cur_block;
						}
					}
				}

				y_f_block_buffer.Dispose();
				z_f_block_buffer.Dispose();

				quad_streams_w.plus_x.EndForEachIndex();
				quad_streams_w.mnus_x.EndForEachIndex();
				quad_streams_w.plus_y.EndForEachIndex();
				quad_streams_w.mnus_y.EndForEachIndex();
				quad_streams_w.plus_z.EndForEachIndex();
				quad_streams_w.mnus_z.EndForEachIndex();

				//2. Greedy Mesh
				ContainerListsToSets<int2>(quad_streams, out var quad_sets);
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

				Greedy_LineSetToRectSet<X_Line_Walker>(coordinate, line_sets.plus_x, rect_sets.plus_x);
				Greedy_LineSetToRectSet<X_Line_Walker>(coordinate, line_sets.mnus_x, rect_sets.mnus_x);
				Greedy_LineSetToRectSet<Y_Line_Walker>(coordinate, line_sets.plus_y, rect_sets.plus_y);
				Greedy_LineSetToRectSet<Y_Line_Walker>(coordinate, line_sets.mnus_y, rect_sets.mnus_y);
				Greedy_LineSetToRectSet<Z_Line_Walker>(coordinate, line_sets.plus_z, rect_sets.plus_z);
				Greedy_LineSetToRectSet<Z_Line_Walker>(coordinate, line_sets.mnus_z, rect_sets.mnus_z);

				quad_streams.Dispose();
				line_sets.Dispose();

			}

			public GenRectSetsJob(
				int inside_block_group, NativeArray<int> block_group_by_id,
				int3 chunk_size,
				NativeArray<ushort> block_chunk,
				NativeArray<ushort> block_chunk_forward_x,
				NativeArray<ushort> block_chunk_forward_y,
				NativeArray<ushort> block_chunk_forward_z,
				out Container6Dir<NativeParallelHashSet<int3>> rect_sets)
			{
				coordinate = new(chunk_size);
				rect_sets = new()
				{
					plus_x = new(1, Allocator.Persistent),
					mnus_x = new(1, Allocator.Persistent),
					plus_y = new(1, Allocator.Persistent),
					mnus_y = new(1, Allocator.Persistent),
					plus_z = new(1, Allocator.Persistent),
					mnus_z = new(1, Allocator.Persistent)
				};
				this.inside_block_group = inside_block_group;
				this.block_group_by_id = block_group_by_id;
				this.block_chunk = block_chunk;
				this.block_chunk_forward_x = block_chunk_forward_x;
				this.block_chunk_forward_y = block_chunk_forward_y;
				this.block_chunk_forward_z = block_chunk_forward_z;
				this.rect_sets = rect_sets;
			}
		}
	}
}