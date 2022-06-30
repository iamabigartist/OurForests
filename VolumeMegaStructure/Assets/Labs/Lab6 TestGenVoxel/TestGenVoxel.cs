using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.DataDefinition.Mesh;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
using VolumeMegaStructure.Generate.Volume;
using VolumeMegaStructure.Manage;
using static VolumeMegaStructure.Util.VectorUtil;
namespace Labs.Lab6_TestGenVoxel
{
	public class TestGenVoxel : MonoBehaviour
	{

		DataMatrix<VolumeUnit> volume_matrix;
		DataMatrix<bool> volume_inside_matrix;
		VoxelMesh voxel_mesh;

		MeshFilter mesh_filter;
		MeshRenderer mesh_renderer;

		void OnEnable()
		{
			volume_matrix = new(int3_one * 100, Allocator.Persistent);
			// volume_matrix.GenerateSphere01(new(1), new(0), 25, new(50, 50, 50));
			// volume_matrix.GenerateRandom01(0.6f, new(1), new(0));
			// for (int i = 0; i < 100; i++)
			// {
			// 	volume_matrix[i, i, i] = new(1);
			// }

			volume_matrix.GenerateCoherentNoise01(0.6f, new(1), new(0), "Hello Voxel the 3rd time");

			volume_inside_matrix = new(volume_matrix.size, Allocator.Persistent);
			voxel_mesh = new(volume_matrix, volume_inside_matrix);
			var empty_check_job = new VolumeMatrixEmptyCheckInside()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix
			};
			empty_check_job.Schedule(volume_matrix.Count, 1024).Complete();
			voxel_mesh.InitGenerate();


			mesh_filter = GetComponent<MeshFilter>();
			mesh_filter.mesh = voxel_mesh.unity_mesh;

			mesh_renderer = GetComponent<MeshRenderer>();
			mesh_renderer.material = MainManager.voxel_render_manager.material;
		}

		void OnDisable()
		{
			voxel_mesh.Dispose();
			volume_matrix.Dispose();
			volume_inside_matrix.Dispose();
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