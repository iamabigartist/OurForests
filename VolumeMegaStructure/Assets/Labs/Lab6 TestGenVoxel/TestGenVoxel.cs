using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.Mesh;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
using VolumeMegaStructure.Manage;
using VolumeMegaStructure.Util;
using static VolumeMegaStructure.Generate.Volume.VolumeMatrixGeneration;
namespace Labs.Lab6_TestGenVoxel
{

	public class TestGenVoxel : MonoBehaviour
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

		void Generate()
		{
			stop_watch.StartRecord("Generate Terrain");
			volume_matrix = new(chunk_size, Allocator.Persistent);
			volume_matrix.GenerateGrassSnowTerrain(MyTerrainParams);
			stop_watch.StopRecord();

			stop_watch.StartRecord("Generate Mesh");
			volume_inside_matrix = new(volume_matrix.size, Allocator.Persistent);
			voxel_mesh = new(volume_matrix, volume_inside_matrix);
			var empty_check_job = new VolumeMatrixEmptyCheckInside()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix
			};
			empty_check_job.Schedule(volume_matrix.Count, 1024).Complete();
			voxel_mesh.InitGenerate();
			stop_watch.StopRecord();

			stop_watch.StartRecord("Bind Components");
			mesh_filter = GetComponent<MeshFilter>();
			mesh_filter.mesh = voxel_mesh.unity_mesh;

			mesh_renderer = GetComponent<MeshRenderer>();
			mesh_renderer.material = MainManager.voxel_render_manager.material;
			stop_watch.StopRecord();

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