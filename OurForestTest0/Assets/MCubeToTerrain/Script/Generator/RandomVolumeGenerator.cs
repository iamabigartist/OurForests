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

        private Vector3 _original_point;

        /// <summary>
        /// The volume scale in three dimension
        /// </summary>
        private Vector3 _scale;

        /// <summary>
        /// The step to put one particle
        /// </summary>
        private float _step;

        #endregion Input

        #region Interface

        /// <summary>
        /// </summary>
        /// <param name="original_point"></param>
        /// <param name="scale">The volume scale in three dimension</param>
        /// <param name="step">The step to put one particle</param>
        public void Input ( Vector3 original_point , Vector3 scale , float step )
        {
            _original_point = original_point;
            _scale = scale;
            _step = step;
        }

        public void Output ( out VolumeMatrix volumeMatrix )
        {
            Vector3Int size = Vector3Int.FloorToInt( _scale / _step );
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