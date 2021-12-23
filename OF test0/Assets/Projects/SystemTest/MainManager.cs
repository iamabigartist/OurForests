using UnityEngine;
using VolumeTerra;
using VolumeTerra.DataDefinition;
namespace SystemTest
{
    /// <summary>
    ///     The programmer custom main function of the game
    /// </summary>
    public static class MainManager
    {

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.SubsystemRegistration )]
        static void GameMain()
        {
            TerrainManager.Init();
            VoxelUVGeneration.Init();
        }

    }
}
