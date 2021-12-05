using UnityEngine;
namespace MCubeToTerrain.Script.Generator
{
    /// <summary>
    /// This class uses 3d perlin noise to generate a <see cref="VolumeMatrix"/>
    /// </summary>
    public class Perlin3DGenerator
    {
        #region Interface

        public void Generate ( out VolumeMatrix volumeMatrix )
        {
            volumeMatrix = new VolumeMatrix( new Vector3Int( 1 , 1 , 1 ) );
        }

        #endregion Interface
    }
}