using System;
using UnityEngine;
namespace VolumeTerra
{
    /// <summary>
    ///     A 3D matrix which represents particles align to uniform grid in a rectangular.
    /// </summary>
    ///<typeparam name="TData">It should be A Serializable Type.</typeparam>
    ///<remarks>The core data type of VolumeTerra</remarks>
    [Serializable]
    public class VolumeMatrix<TData>
    {
        public TData[] data;

        public Vector3Int size;
        
        public VolumeMatrix(Vector3Int size)
        {
            this.size = size;
            this.data = new TData[size.x * size.y * size.z];
        }

        public TData this[int x, int y, int z]
        {
            get => this.data[x + y * this.size.x + z * this.size.y * this.size.x];
            set => this.data[x + y * this.size.x + z * this.size.y * this.size.x] = value;
        }

        public TData this[Vector3Int idx]
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
    }
}