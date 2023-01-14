using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
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

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }
			var cur_inside = volume_inside_matrix[volume_i];

			var x_forward = new int3(x + 1, y, z);
			var y_forward = new int3(x, y + 1, z);
			var z_forward = new int3(x, y, z + 1);
			var x_f_inside = volume_inside_matrix[x_forward];
			var y_f_inside = volume_inside_matrix[y_forward];
			var z_f_inside = volume_inside_matrix[z_forward];

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
			DataMatrix<bool> volume_inside_matrix,
			out NativeQueue<int>[] streams,
			JobHandle deps = default)
		{
			int volume_count = volume_inside_matrix.Count;
			streams = new NativeQueue<int>[6];
			for (int i = 0; i < 6; i++) { streams[i] = new(Allocator.TempJob); }
			var job = new GenDirectionQuadQueue()
			{
				volume_inside_matrix = volume_inside_matrix,
				stream_x = streams[0].AsParallelWriter(),
				stream_x_minus = streams[1].AsParallelWriter(),
				stream_y = streams[2].AsParallelWriter(),
				stream_y_minus = streams[3].AsParallelWriter(),
				stream_z = streams[4].AsParallelWriter(),
				stream_z_minus = streams[5].AsParallelWriter()
			};
			return job.ScheduleParallel(volume_count,
				GetBatchSize_SquareThreadCount(volume_count), deps);
		}
	}
}