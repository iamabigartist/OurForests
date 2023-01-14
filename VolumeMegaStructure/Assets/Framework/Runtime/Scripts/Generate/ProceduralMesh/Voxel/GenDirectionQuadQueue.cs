using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using VolumeMegaStructure.DataDefinition.Container;
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

		public void Execute(int volume_i)
		{
			volume_inside_matrix.PositionByIndex(volume_i, out var x, out var y, out var z);
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }
			var cur_inside = volume_inside_matrix[volume_i];

			var x_f_inside = volume_inside_matrix[x + 1, y, z];
			var y_f_inside = volume_inside_matrix[x, y + 1, z];
			var z_f_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_inside != x_f_inside)
			{
				if (cur_inside) { stream_x.Enqueue(volume_i); }
				else { stream_x_minus.Enqueue(volume_i); }
			}

			if (cur_inside != y_f_inside)
			{
				if (cur_inside) { stream_y.Enqueue(volume_i); }
				else { stream_y_minus.Enqueue(volume_i); }
			}

			if (cur_inside != z_f_inside)
			{
				if (cur_inside) { stream_z.Enqueue(volume_i); }
				else { stream_z_minus.Enqueue(volume_i); }
			}
		}

		static int GetBatchSize_SquareThreadCount(int total)
		{
			float pow = math.pow(JobsUtility.MaxJobThreadCount, 2);
			return (int)math.ceil(total / pow);
		}

		public static JobHandle ScheduleParallel(
			int volume_count,
			DataMatrix<bool> volume_inside_matrix,
			out NativeQueue<int>[] queues,
			JobHandle deps = default)
		{
			queues = new NativeQueue<int>[6];
			for (int i = 0; i < 6; i++) { queues[i] = new(Allocator.TempJob); }
			var job = new GenDirectionQuadQueue()
			{
				volume_inside_matrix = volume_inside_matrix,
				stream_x = queues[0].AsParallelWriter(),
				stream_x_minus = queues[1].AsParallelWriter(),
				stream_y = queues[2].AsParallelWriter(),
				stream_y_minus = queues[3].AsParallelWriter(),
				stream_z = queues[4].AsParallelWriter(),
				stream_z_minus = queues[5].AsParallelWriter()
			};
			return job.ScheduleParallel(volume_count,
				GetBatchSize_SquareThreadCount(volume_count), deps);
		}
	}
}