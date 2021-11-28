using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace MarchingCube1
{

    /// <summary>
    ///     A uniform partice filed
    /// </summary>
    [Serializable]
    public class VolumeMatrix
    {
        public float[] data;

        [SerializeField]
        public Vector3Int volume_size;
        public Vector3Int cube_size;

        public VolumeMatrix(Vector3Int volumeSize)
        {
            volume_size = volumeSize;
            cube_size = new Vector3Int( volume_size.x - 1, volume_size.y - 1, volume_size.z - 1 );
            data = new float[volumeSize.x * volumeSize.y * volumeSize.z];
        }

        public float this[int x, int y, int z]
        {
            get => data[x + y * volume_size.x + z * volume_size.y * volume_size.x];
            set => data[x + y * volume_size.x + z * volume_size.y * volume_size.x] = value;
        }

        public float this[Vector3Int idx]
        {
            get => data[idx.x + idx.y * volume_size.x + idx.z * volume_size.y * volume_size.x];
            set => data[idx.x + idx.y * volume_size.x + idx.z * volume_size.y * volume_size.x] = value;
        }

        public int Count => volume_size.x * volume_size.y * volume_size.z;

        public int CubeCount => cube_size.x * cube_size.y * cube_size.z;

        public int Index(int x, int y, int z)
        {
            return x + y * volume_size.x + z * volume_size.y * volume_size.x;
        }

        public void SaveToFile(string path)
        {
            var formatter = new BinaryFormatter();
            var fs = new FileStream( path, FileMode.Create );
            formatter.Serialize( fs, volume_size );
            formatter.Serialize( fs, data );
            fs.Close();
            Debug.Log( "volume data saved to " + path );
        }

        [SuppressMessage( "ReSharper", "PossibleNullReferenceException" )]
        public static VolumeMatrix LoadFromFile(string path)
        {
            if (!File.Exists( path ))
            {
                Debug.LogError( "No volume data found in " + path );
                return null;
            }
            var formatter = new BinaryFormatter();
            var fs = new FileStream( path, FileMode.Open );
            var int3 = (Vector3Int)formatter.Deserialize( fs );
            var ret = new VolumeMatrix( new Vector3Int( int3.x, int3.y, int3.z ) )
                { data = formatter.Deserialize( fs ) as float[] };
            fs.Close();
            Debug.Log( "Volume data loaded from " + path );
            return ret;
        }
    }

    /// <summary>
    ///     A Marching Cube mesh with its generation info
    /// </summary>
    public class MarchingCubeMesh
    {
    #region Config

        public float iso_value;

    #endregion Config

    #region Data

        public Mesh mesh;

    #endregion Data

    #region Input

        public VolumeMatrix volume_matrix;

    #endregion Input

    #region Info

        /// <summary>
        ///     Matrix marking every particle whether it is bigger than the isovalue
        /// </summary>
        public bool[] mark_matrix;

        /// <summary>
        ///     Array that store the actual index of each vertex on the edges
        /// </summary>
        public int[] vertices_indices;

        /// <summary>
        ///     The start indices of the 3 axis edges' vertices
        /// </summary>
        public Vector3Int start_indices;

    #endregion Info
    }
}
