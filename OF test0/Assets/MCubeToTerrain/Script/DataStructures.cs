using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using UnityEngine;

namespace MarchingCube1
{
    public class Particles
    {
        public readonly int size;
        public Vector3[] data;
    }

    [System.Serializable]
    public class Dimension
    {
        public int x;
        public int y;
        public int z;

        public Dimension ( Vector3Int int3 )
        {
            x = int3.x;
            y = int3.y;
            z = int3.z;
        }
    }

    /// <summary>
    /// A uniform partice filed
    /// </summary>
    [System.Serializable]
    public class VolumeMatrix
    {
        public readonly Vector3Int size;
        public float[] data;

        public VolumeMatrix ( Vector3Int size )
        {
            this.size = size;
            data = new float[ size.x * size.y * size.z ];
        }

        public float this[ int x , int y , int z ]
        {
            get => data[ x + y * size.x + z * size.y * size.x ];
            set
            {
                data[ x + y * size.x + z * size.y * size.x ] = value;
            }
        }

        public float this[ Vector3Int idx ]
        {
            get => data[ idx.x + idx.y * size.x + idx.z * size.y * size.x ];
            set
            {
                data[ idx.x + idx.y * size.x + idx.z * size.y * size.x ] = value;
            }
        }

        public int count => size.x * size.y * size.z;

        public int voxel_count => ( size.x - 1 ) * ( size.y - 1 ) * ( size.z - 1 );

        public int index ( int x , int y , int z )
        {
            return x + y * size.x + z * size.y * size.x;
        }

        public void SaveToFile ( string path )
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream( path , FileMode.Create );
            formatter.Serialize( fs , new Dimension( size ) );
            formatter.Serialize( fs , data );
            fs.Close();
            Debug.Log( "volume data saved to " + path );
        }

        public static VolumeMatrix LoadFromFile ( string path )
        {
            if ( !File.Exists( path ) )
            {
                Debug.LogError( "No volume data found in " + path );
                return null;
            }
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream( path , FileMode.Open );
            Dimension int3 = formatter.Deserialize( fs ) as Dimension;
            VolumeMatrix ret = new VolumeMatrix( new Vector3Int( int3.x , int3.y , int3.z ) );
            ret.data = formatter.Deserialize( fs ) as float[];
            fs.Close();
            Debug.Log( "Volume data loaded from " + path );
            return ret;
        }
    }

    /// <summary>
    /// A Marching Cube mesh with its generation info
    /// </summary>
    public class MarchingCubeMesh
    {
        #region Input

        public VolumeMatrix volume_matrix;

        #endregion Input

        #region Config

        public float iso_value;

        #endregion Config

        #region Info

        /// <summary>
        /// Matrix marking every particle whether it is bigger than the isovalue
        /// </summary>
        public bool[] mark_matrix;

        /// <summary>
        /// Array that store the actual index of each vertex on the edges
        /// </summary>
        public int[] vertices_indices;

        /// <summary>
        /// The start indices of the 3 axis edges' vertices
        /// </summary>
        public Vector3Int start_indices;

        #endregion Info

        #region Data

        public Mesh mesh;

        #endregion Data
    }
}