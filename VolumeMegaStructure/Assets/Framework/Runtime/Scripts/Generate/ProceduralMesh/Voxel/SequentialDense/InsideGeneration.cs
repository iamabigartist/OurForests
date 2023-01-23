using PrototypePackages.JobUtils.Template;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	public static class InsideGeneration
	{
		public struct GenInsideJob : IJob, IPlan
		{
			int cur_block_group;
			[NoAlias] [ReadOnly] NativeArray<int> block_id_to_block_group;
			[NoAlias] [ReadOnly] NativeArray<ushort> volume_chunk;
			[NoAlias] [WriteOnly] NativeArray<bool> inside_chunk;
			public void Execute()
			{
				for (int i = 0; i < volume_chunk.Length; i++)
				{
					inside_chunk[i] = block_id_to_block_group[volume_chunk[i]] == cur_block_group;
				}
			}
			public GenInsideJob(int cur_block_group, NativeArray<int> block_id_to_block_group,
				NativeArray<ushort> volume_chunk, out NativeArray<bool> inside_chunk)
			{
				inside_chunk = new(volume_chunk.Length, Allocator.TempJob);
				this.cur_block_group = cur_block_group;
				this.block_id_to_block_group = block_id_to_block_group;
				this.volume_chunk = volume_chunk;
				this.inside_chunk = inside_chunk;
			}
		}
	}
}