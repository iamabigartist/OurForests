using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.Mesh;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense;
using VolumeMegaStructure.Manage;
using VolumeMegaStructure.Util;
using static VolumeMegaStructure.Generate.Volume.VolumeMatrixGeneration;
namespace Labs.TestGenVoxel
{
	public class TestGenVoxelParallelDense : MonoBehaviour
	{

		DataMatrix<ushort> volume_matrix;
		DataMatrix<bool> volume_inside_matrix;
		VoxelMesh voxel_mesh;

		MeshFilter mesh_filter;
		MeshRenderer mesh_renderer;

		public int3 chunk_size;
		public GenerateGrassSnowTerrainParams MyTerrainParams;

		ProfileStopWatch stop_watch;

		void Clear()
		{
			voxel_mesh?.Dispose();
			volume_matrix.Dispose();
			volume_inside_matrix.Dispose();

			stop_watch.Clear();
		}

		async void Generate()
		{
			stop_watch.Start("Generate Terrain");
			volume_matrix = new(chunk_size, Allocator.Persistent);
			volume_matrix.GenerateGrassSnowTerrain(MyTerrainParams);
			stop_watch.Stop();

			stop_watch.Start("CheckEmpty");
			volume_inside_matrix = new(volume_matrix.size, Allocator.Persistent);
			var empty_check_job = new VolumeMatrixEmptyCheckInside()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix
			};
			empty_check_job.Schedule(volume_matrix.Count, 1024).Complete();
			stop_watch.Stop();

			stop_watch.Start("Generate Mesh");
			voxel_mesh = new(volume_matrix, volume_inside_matrix);
			// voxel_mesh.InitGenerate();
			await voxel_mesh.InitGenerate_Greedy();
			stop_watch.Stop();

			stop_watch.Start("Bind Components");
			mesh_filter = GetComponent<MeshFilter>();
			mesh_filter.mesh = voxel_mesh.unity_mesh;

			mesh_renderer = GetComponent<MeshRenderer>();
			mesh_renderer.material = MainManager.voxel_render_manager.material;
			stop_watch.Stop();

			Debug.Log(stop_watch.PrintAllRecords());

		}

		[ContextMenu("Regenerate Terrain")]
		public void Regenerate()
		{
			Clear();
			Generate();
		}


		void OnEnable()
		{
			stop_watch = new();
			Generate();
		}
		void OnDestroy()
		{
			Clear();
		}

		void OnDrawGizmos()
		{
			int size = 3;
			for (int z = 0; z < size; z++)
			{
				for (int y = 0; y < size; y++)
				{
					for (int x = 0; x < size; x++)
					{
						Gizmos.DrawSphere(new(x, y, z), 0.125f);
					}
				}

			}
		}
	}
}