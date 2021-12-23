using UnityEngine;
using VolumeTerra.DataDefinition;
namespace SystemTest
{
    /// <summary>
    ///     The programmer custom main function of the game
    /// </summary>
    public static class MainManager
    {

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded )]
        static void GameMain()
        {
            TerrainManager.Init();
            VoxelUVGeneration.Init();
        }

    }
}
