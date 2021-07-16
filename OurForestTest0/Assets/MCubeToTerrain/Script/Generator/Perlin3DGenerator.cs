using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MarchingCube1
{
    /// <summary>
    /// This class uses 3d perlin noise to generate a <see cref="VolumeMatrix"/>
    /// </summary>
    public class Perlin3DGenerator
    {
        #region Interface

        public void Generate(out VolumeMatrix volumeMatrix)
        {
            volumeMatrix = new VolumeMatrix(new Vector3Int(1, 1, 1));
        }

        #endregion Interface
    }
}