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
		public static VoxelSourceTables voxel_source_tables { get; private set; }
		public static IDManager id_manager { get; private set; }
		public static VoxelRenderManager voxel_render_manager { get; private set; }

	#endregion

	#region GameMainProcess

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void GameMainInit()
		{
			voxel_rotation_face_table = new();
			voxel_source_tables = new();
			id_manager = new();
			voxel_render_manager = new(voxel_source_tables);
		}

		public static void TerminateManagers()
		{
			voxel_source_tables.Dispose();
			voxel_render_manager.Dispose();
		}

	#endregion


	}
}