using PrototypePackages.PrototypeUtils;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense;
using static PrototypePackages.JobUtils.Template.IPlan;
using static PrototypePackages.PrototypeUtils.AsyncUtil;
using static Unity.Jobs.JobHandle;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense.MeshGeneration;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense.RectsGeneration;
using static VolumeMegaStructure.Manage.MainManager;
namespace VolumeMegaStructure.Generate.WorldData
{
	public static class WorldDataGeneration
	{
		public static async void GenChunkBlocks(this Chunk chunk) {}
		public static async void GenChunkMesh(this Chunk chunk, int cur_block_group)
		{
			var world = chunk.world;
			var chunk_size = world.chunk_size;
			var cur_pos = chunk.world_position;
			var xf_pos = cur_pos + (1, 0, 0).i3();
			var yf_pos = cur_pos + (0, 1, 0).i3();
			var zf_pos = cur_pos + (0, 0, 1).i3();

			//Ensure the 4 chunks' blocks have been planned and get the job handle.
			var block_gen_jh_cur = chunk.blocks_gen_jh;
			var block_gen_jh_xf = world[xf_pos].blocks_gen_jh;
			var block_gen_jh_yf = world[yf_pos].blocks_gen_jh;
			var block_gen_jh_zf = world[zf_pos].blocks_gen_jh;
			var block_gen_jh = CombineDependencies(block_gen_jh_cur,
				CombineDependencies(block_gen_jh_xf, block_gen_jh_yf, block_gen_jh_zf));
			var block_chunk_cur = chunk.blocks;
			var block_chunk_xf = world[xf_pos].blocks;
			var block_chunk_yf = world[yf_pos].blocks;
			var block_chunk_zf = world[zf_pos].blocks;

			//Plan gen_rect_set on previous jh.
			var rect_jh = Plan<GenRectSetsJob>(new(
				cur_block_group, block_manager.block_group_by_id, chunk_size,
				block_chunk_cur, block_chunk_xf, block_chunk_yf, block_chunk_zf,
				out var rect_sets), block_gen_jh);

			//Wait for jh complete.
			ScheduleBatchedJobs();
			await WaitUntil(() => rect_jh.IsCompleted, 1);
			rect_jh.Complete();

			//GenMeshAsync and set mesh.
			chunk.mesh = await GenMeshAsync(voxel_render_manager.max_index_array, chunk_size, rect_sets);

			//Clean
			rect_sets.Dispose();
		}
	}
}