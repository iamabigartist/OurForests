using PrototypePackages.JobUtils.Template;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static VolumeMegaStructure.Util.JobSystem.ScheduleUtils;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	public static class IndexGeneration
	{
		[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
		public struct GenIndexArray : IJobFor, IPlanFor
		{
			public int length => rect_count;
			public int batch => GetBatchSize_WorkerThreadCount(length);

			[NoAlias] [ReadOnly] NativeArray<int> local_vertex_index;
			[NoAlias] int rect_count;
			[NoAlias] [NativeDisableParallelForRestriction] [WriteOnly] NativeArray<int> index_array;
			public void Execute(int i_rect)
			{
				int render_i0 = i_rect * 6;
				int vertex_i0 = i_rect * 4;

				for (int local_render_i = 0; local_render_i < 6; local_render_i++)
				{
					int render_i = render_i0 + local_render_i;
					int vertex_i = local_vertex_index[local_render_i] + vertex_i0;
					index_array[render_i] = vertex_i;
				}
			}

			public GenIndexArray(NativeArray<int> local_vertex_index, int rect_count, out NativeArray<int> index_array)
			{
				index_array = new(rect_count * 6, Allocator.Persistent);
				this.local_vertex_index = local_vertex_index;
				this.rect_count = rect_count;
				this.index_array = index_array;
			}
		}
	}
}