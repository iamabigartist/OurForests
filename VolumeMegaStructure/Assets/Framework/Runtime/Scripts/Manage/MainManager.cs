using UnityEngine;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
namespace VolumeMegaStructure.Manage
{
	/// <summary>
	///     The programmer custom main function of the game
	/// </summary>
	public static class MainManager
	{

	#region Configs

		public const string SOURCE_CUBE_PATH = "UVCubeShow-m_mesh";

	#endregion

	#region States

	#endregion

	#region GlobalTools

		public static VoxelRotationFaceTable voxel_rotation_face_table { get; private set; }
		public static VoxelGenSourceTables voxel_gen_source_tables { get; private set; }

	#endregion

	#region GameMainProcess

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void GameMainInit()
		{
			voxel_rotation_face_table = new();
			voxel_gen_source_tables = new();
		}

		public static void TerminateManager()
		{
			voxel_gen_source_tables.Dispose();
		}

	#endregion


	}
}