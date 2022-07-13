using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     在扫描VolumeMatrix之前，是不知道一共有多少个quad需要生成的，而最多可能有3*n*n*(n+1)个quad，这个数量很大，因此使用冗余的数组作为最终quad结果的存放内存很不划算。然而在扫描之后，使用NativeCounter统计quad数量，再以此数量分配后续各种数组的长度。
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenQuadCount : IJobFor
	{
		[ReadOnly] public DataMatrix<VolumeUnit> volume_matrix;
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeCounter.ParallelWriter quad_counter;
		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_block_inside != x_forward_inside)
			{
				quad_counter.Increment();
			}

			if (cur_block_inside != y_forward_inside)
			{
				quad_counter.Increment();
			}

			if (cur_block_inside != z_forward_inside)
			{
				quad_counter.Increment();
			}
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<VolumeUnit> volume_matrix,
			DataMatrix<bool> volume_inside_matrix,
			out NativeCounter quad_counter,
			JobHandle deps = default)
		{
			quad_counter = new(Allocator.TempJob);
			var job = new GenQuadCount()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix,
				quad_counter = quad_counter
			};
			return job.ScheduleParallel(volume_matrix.Count, 1, deps);
		}
	}
}