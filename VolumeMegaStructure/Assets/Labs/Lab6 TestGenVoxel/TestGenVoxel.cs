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
namespace Labs.Lab6_TestGenVoxel.Editor
{
	public class TestGenVoxel : MonoBehaviour
	{

		DataMatrix<VolumeUnit> volume_matrix;
		DataMatrix<bool> volume_inside_matrix;
		VoxelMesh voxel_mesh;

		MeshFilter mesh_filter;

		void OnEnable()
		{
			MainManager.GameMainInit();
			volume_matrix = new(int3_one * 400, Allocator.Persistent);
			// volume_matrix.GenerateSphere01(new(1), new(0), 25, new(50, 50, 50));
			// volume_matrix.GenerateRandom01(0.6f, new(1), new(0));
			volume_matrix.GenerateCoherentNoise01(0.6f, new(1), new(0), "Hello Voxel the 3rd time");
			// for (int i = 0; i < 100; i++)
			// {
			// 	volume_matrix[i, i, i] = new(1);
			// }
			// volume_matrix[0, 0, 0] = new(1);
			// volume_matrix[4, 4, 4] = new(1);
			// volume_matrix[4, 5, 4] = new(1);
			// volume_matrix[4, 4, 5] = new(1);
			// volume_matrix[4, 5, 5] = new(1);

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

		}

		void OnDisable()
		{
			voxel_mesh.Dispose();
			volume_matrix.Dispose();
			volume_inside_matrix.Dispose();
			MainManager.TerminateManagers();
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