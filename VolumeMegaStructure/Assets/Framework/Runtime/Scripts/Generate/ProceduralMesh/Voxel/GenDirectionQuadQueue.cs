using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     统计所有quad的必要信息，以随机顺序组成一个列表，供后面的数组赋值操作时候参考位置。
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenDirectionQuadQueue : IJobFor
	{
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeQueue<int>.ParallelWriter stream_x;
		[WriteOnly] public NativeQueue<int>.ParallelWriter stream_x_minus;
		[WriteOnly] public NativeQueue<int>.ParallelWriter stream_y;
		[WriteOnly] public NativeQueue<int>.ParallelWriter stream_y_minus;
		[WriteOnly] public NativeQueue<int>.ParallelWriter stream_z;
		[WriteOnly] public NativeQueue<int>.ParallelWriter stream_z_minus;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void WriteQuadToStream(int i, ref NativeQueue<int>.ParallelWriter stream)
		{
			stream.Enqueue(i);
		}

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_block_inside != x_forward_inside)
			{
				if (cur_block_inside) { WriteQuadToStream(volume_i, ref stream_x); }
				else { WriteQuadToStream(volume_i, ref stream_x_minus); }
			}

			if (cur_block_inside != y_forward_inside)
			{
				if (cur_block_inside) { WriteQuadToStream(volume_i, ref stream_y); }
				else { WriteQuadToStream(volume_i, ref stream_y_minus); }
			}

			if (cur_block_inside != z_forward_inside)
			{
				if (cur_block_inside) { WriteQuadToStream(volume_i, ref stream_z); }
				else { WriteQuadToStream(volume_i, ref stream_z_minus); }
			}
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<bool> volume_inside_matrix,
			out NativeQueue<int> stream_x,
			out NativeQueue<int> stream_x_minus,
			out NativeQueue<int> stream_y,
			out NativeQueue<int> stream_y_minus,
			out NativeQueue<int> stream_z,
			out NativeQueue<int> stream_z_minus,
			JobHandle deps = default)
		{
			int volume_count = volume_inside_matrix.Count;
			stream_x = new(Allocator.TempJob);
			stream_x_minus = new(Allocator.TempJob);
			stream_y = new(Allocator.TempJob);
			stream_y_minus = new(Allocator.TempJob);
			stream_z = new(Allocator.TempJob);
			stream_z_minus = new(Allocator.TempJob);
			var job = new GenDirectionQuadQueue()
			{
				volume_inside_matrix = volume_inside_matrix,
				stream_x = stream_x.AsParallelWriter(),
				stream_x_minus = stream_x_minus.AsParallelWriter(),
				stream_y = stream_y.AsParallelWriter(),
				stream_y_minus = stream_y_minus.AsParallelWriter(),
				stream_z = stream_z.AsParallelWriter(),
				stream_z_minus = stream_z_minus.AsParallelWriter()
			};
			return job.ScheduleParallel(volume_inside_matrix.Count, 1024, deps);
		}
	}
}