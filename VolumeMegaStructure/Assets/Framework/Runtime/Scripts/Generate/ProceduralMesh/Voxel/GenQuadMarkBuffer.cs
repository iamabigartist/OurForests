using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Util;
using static VolumeMegaStructure.Util.JobSystem.ScheduleUtils;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     统计所有quad的必要信息，以随机顺序组成一个列表，供后面的数组赋值操作时候参考位置。
	/// </summary>
	// [BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenQuadMarkBuffer : IJobFor
	{
		[NativeSetThreadIndex] int thread_id;
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public UnsafeList<UnsafeList<UnsafeAppendBuffer>> buffers;

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// void WriteQuadToStream(int i, ref UnsafeAppendBuffer stream)
		// {
		// 	stream.BeginForEachIndex();
		// 	stream.Add(i);
		// }

		public void Execute(int volume_i)
		{
			var cur_thread_buffers = buffers[thread_id];
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_block_inside != x_forward_inside)
			{
				if (cur_block_inside) { cur_thread_buffers[0].Add(volume_i); }
				else { cur_thread_buffers[1].Add(volume_i); }
			}

			if (cur_block_inside != y_forward_inside)
			{
				if (cur_block_inside) { cur_thread_buffers[2].Add(volume_i); }
				else { cur_thread_buffers[3].Add(volume_i); }
			}

			if (cur_block_inside != z_forward_inside)
			{
				if (cur_block_inside) { cur_thread_buffers[4].Add(volume_i); }
				else { cur_thread_buffers[5].Add(volume_i); }
			}
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<bool> volume_inside_matrix,
			out UnsafeList<UnsafeList<UnsafeAppendBuffer>> buffers,
			JobHandle deps = default)
		{
			int volume_count = volume_inside_matrix.Count;
			int max_worker_count = JobsUtility.MaxJobThreadCount;
			buffers = new(max_worker_count, Allocator.TempJob);
			for (int i = 0; i < max_worker_count; i++)
			{
				var cur_buffer = new UnsafeList<UnsafeAppendBuffer>(6, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				for (int j = 0; j < 6; j++)
				{
					cur_buffer.Add(new(1, sizeof(int), Allocator.TempJob));
				}
				buffers.Add(cur_buffer);
			}
			var job = new GenQuadMarkBuffer()
			{
				volume_inside_matrix = volume_inside_matrix,
				buffers = buffers
			};
			return job.ScheduleParallel(volume_count, GetBatchSize_MaxThreadCount(volume_count), deps);
		}
	}
}