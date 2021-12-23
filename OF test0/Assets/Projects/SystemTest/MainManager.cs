using UnityEngine;
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
            Debug.Log( "Main!" );
            aaa();
        }

        static void aaa()
        {
            TerrainManager.Init();
        }

    }
}
