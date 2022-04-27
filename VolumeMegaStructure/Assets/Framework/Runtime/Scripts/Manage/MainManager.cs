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

        public static VoxelRotationFaceTable VoxelRotationFaceTable { get; private set; }

    #endregion

    #region GameMainProcess

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
        static void GameMainInit()
        {
            VoxelRotationFaceTable = new();
        }

    #endregion


    }
}
