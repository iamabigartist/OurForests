using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static PrototypePackages.JobUtils.Template.IPlan;
using static PrototypePackages.PrototypeUtils.AsyncUtil;
using static Unity.Jobs.JobHandle;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense.InsideGeneration;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense.MeshGeneration;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense.RectsGeneration;
using static VolumeMegaStructure.Generate.Volume.Terrain.TestTerrainGeneration;
using static VolumeMegaStructure.Manage.MainManager;
using static VolumeMegaStructure.Util.DebugUtils;
namespace Labs.TestGenVoxel
{
	public class TestGenVoxelSequentialDense : MonoBehaviour
	{
		int3 chunk_size = new(256, 256, 256);
		NativeArray<int> block_id_to_block_group = new(new[]
		{
			0,
			1, 1, 1, 1
		}, Allocator.Persistent);
		async void GenSingleOnce()
		{
			var w = new ProfileStopWatch();

			w.Start("Gen 4 volume chunks terrain");
			var terrain_jh0 = Plan<GenSimpleMountainTerrainJob>(
				new(3, 4, 2, 1, 0, 140, 160,
					new(0, 0),
					new(0.001f, 0),
					new(new(8, 0), new(250, 0), new(1000)),
					new(new(60, 0), new(3, 0), new(1000)),
					chunk_size, out var volume_chunk));
			var terrain_jh_x = Plan<GenSimpleMountainTerrainJob>(
				new(3, 4, 2, 1, 0, 140, 160,
					new(256, 0),
					new(0.001f, 0),
					new(new(8, 0), new(250, 0), new(1000)),
					new(new(60, 0), new(3, 0), new(1000)),
					chunk_size, out var volume_chunk_x_f));
			var terrain_jh_z = Plan<GenSimpleMountainTerrainJob>(
				new(3, 4, 2, 1, 0, 140, 160,
					new(0, 256),
					new(0.001f, 0),
					new(new(8, 0), new(250, 0), new(1000)),
					new(new(60, 0), new(3, 0), new(1000)),
					chunk_size, out var volume_chunk_z_f));
			ScheduleBatchedJobs();
			var volume_chunk_y_f = new NativeArray<ushort>(Enumerable.Repeat((ushort)0, chunk_size.volume()).ToArray(), Allocator.Persistent);
			var terrain_jh = CombineDependencies(terrain_jh0, terrain_jh_x, terrain_jh_z);
			await WaitUntil(() => terrain_jh.IsCompleted, 1);
			terrain_jh.Complete();
			w.Stop();

			w.Start("Gen 4 inside chunks");
			var inside_jh0 = Plan<GenInsideJob>(new(1, block_id_to_block_group, volume_chunk, out var inside_chunk));
			var inside_jh_x = Plan<GenInsideJob>(new(1, block_id_to_block_group, volume_chunk_x_f, out var inside_chunk_x_f));
			var inside_jh_z = Plan<GenInsideJob>(new(1, block_id_to_block_group, volume_chunk_z_f, out var inside_chunk_z_f));
			var jh_y = Plan<GenInsideJob>(new(1, block_id_to_block_group, volume_chunk_y_f, out var inside_chunk_y_f));
			ScheduleBatchedJobs();
			var inside_jh = CombineDependencies(inside_jh0, CombineDependencies(inside_jh_x, jh_y, inside_jh_z));
			await WaitUntil(() => inside_jh.IsCompleted, 1);
			inside_jh.Complete();
			w.Stop();

			w.Start("ScanRectSets");
			var rect_jh = Plan<GenRectSetsJob>(new(chunk_size,
				volume_chunk, volume_chunk_x_f, volume_chunk_y_f, volume_chunk_z_f,
				inside_chunk, inside_chunk_x_f, inside_chunk_y_f, inside_chunk_z_f,
				out var rect_sets));
			ScheduleBatchedJobs();
			await WaitUntil(() => rect_jh.IsCompleted, 1);
			rect_jh.Complete();
			w.Stop();

			w.Start("GenMesh");
			var mesh = await GenMeshAsync(voxel_render_manager.max_index_array, chunk_size, rect_sets);
			w.Stop();

			Log(w.PrintAllRecords());
		}
		void Start()
		{
			GenSingleOnce();
		}
	}
}