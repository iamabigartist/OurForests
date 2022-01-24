using UnityEngine;
using VolumeTerra.DataDefinition;
using VolumeTerra.Management;
namespace MyForestSystem
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

        public static TerrainManager TerrainManager { get; private set; }
        public static VoxelUVGenerator VoxelUVGenerator { get; private set; }

    #endregion

    #region GameMainProcess

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
        static void GameMainInit()
        {
            TerrainManager = new TerrainManager();
            VoxelUVGenerator = new VoxelUVGenerator( SOURCE_CUBE_PATH );
        }

    #endregion


    }
}