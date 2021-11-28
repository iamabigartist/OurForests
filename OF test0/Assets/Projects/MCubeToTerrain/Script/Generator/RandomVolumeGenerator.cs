
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MarchingCube1
{
    /// <summary>
    /// This class generate a random <see cref="VolumeMatrix"/>.
    /// </summary>
    public class RandomVolumeGenerator
    {
        #region Input

        /// <summary>
        /// The number of volume in each dimension
        /// </summary>
        private int number;

        #endregion Input

        #region Interface

        /// <summary>
        /// </summary>
        public void Input ( int number )
        {
            this.number = number;
        }

        public void Output ( out VolumeMatrix volumeMatrix )
        {
            Vector3Int size = Vector3Int.one * this.number;
            volumeMatrix = new VolumeMatrix( size );
            for ( int z = 0; z < size.z; z++ )
            {
                for ( int y = 0; y < size.y; y++ )
                {
                    for ( int x = 0; x < size.x; x++ )
                    {
                        volumeMatrix[ x , y , z ] = Random.Range( 0f , 5f );
                    }
                }
            }
        }

        #endregion Interface
    }
}