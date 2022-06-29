using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.DataDefinition.Mesh;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
using VolumeMegaStructure.Generate.Volume;
using static VolumeMegaStructure.Util.VectorUtil;
namespace Labs.Lab6_TestGenVoxel.Editor
{
	public class TestGenVoxelEditor : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab6_TestGenVoxel.Editor/TestGenVoxelEditor")]
		static void ShowWindow()
		{
			var window = GetWindow<TestGenVoxelEditor>();
			window.titleContent = new("TestGenVoxelEditor");
			window.Show();
		}

		DataMatrix<VolumeUnit> volume_matrix;
		DataMatrix<bool> volume_inside_matrix;
		VoxelMesh voxel_mesh;
		void OnEnable()
		{
			volume_matrix = new(int3_one * 100, Allocator.Persistent);
			volume_matrix.GenerateRandom01(0.6f, new(0), new(1));
			volume_inside_matrix = new(volume_matrix.size, Allocator.Persistent);
			voxel_mesh = new(volume_matrix, volume_inside_matrix);
			var empty_check_job = new VolumeMatrixEmptyCheckInside()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix
			};
			empty_check_job.Schedule(volume_matrix.Count, 1024).Complete();
			voxel_mesh.InitGenerate();
		}

		void OnGUI() {}
	}
}