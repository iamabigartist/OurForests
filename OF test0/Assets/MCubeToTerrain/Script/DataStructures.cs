using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// ReSharper disable once CheckNamespace
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
        public Vector3Int size;

        public VolumeMatrix(Vector3Int size)
        {
            this.size = size;
            this.data = new float[size.x * size.y * size.z];
        }

        public float this[int x, int y, int z]
        {
            get => this.data[x + y * this.size.x + z * this.size.y * this.size.x];
            set => this.data[x + y * this.size.x + z * this.size.y * this.size.x] = value;
        }

        public float this[Vector3Int idx]
        {
            get => this.data[idx.x + idx.y * this.size.x + idx.z * this.size.y * this.size.x];
            set => this.data[idx.x + idx.y * this.size.x + idx.z * this.size.y * this.size.x] = value;
        }

        public int Count => this.size.x * this.size.y * this.size.z;

        public int VoxelCount => (this.size.x - 1) * (this.size.y - 1) * (this.size.z - 1);

        public int Index(int x, int y, int z)
        {
            return x + y * this.size.x + z * this.size.y * this.size.x;
        }

        public void SaveToFile(string path)
        {
            var formatter = new BinaryFormatter();
            var fs = new FileStream(path, FileMode.Create);
            formatter.Serialize(fs, this.size);
            formatter.Serialize(fs, this.data);
            fs.Close();
            Debug.Log("volume data saved to " + path);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static VolumeMatrix LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("No volume data found in " + path);
                return null;
            }
            var formatter = new BinaryFormatter();
            var fs = new FileStream(path, FileMode.Open);
            var int3 = (Vector3Int)formatter.Deserialize(fs);
            var ret = new VolumeMatrix(new Vector3Int(int3.x, int3.y, int3.z))
                {data = formatter.Deserialize(fs) as float[]};
            fs.Close();
            Debug.Log("Volume data loaded from " + path);
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