using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.DataDefinition.Mesh;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
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
			volume_matrix = new(int3_one * 100, Allocator.Persistent);
			for (int i = 0; i < 100; i++)
			{
				volume_matrix[i, i, i] = new(1);
			}
			// volume_matrix[2, 2, 2] = new(1);
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
			MainManager.TerminateManagers();
		}
	}
}