using System;
using MUtility;
using UnityEngine;
namespace VolumeTerra.DataDefinition
{
    /// <summary>
    ///     A 3D matrix which represents particles align to uniform grid in a rectangular.
    /// </summary>
    /// <typeparam name="TData">It should be A Serializable Type.</typeparam>
    /// <remarks>The core data type of VolumeTerra</remarks>
    [Serializable]
    public class VolumeMatrix<TData>
    {
        public TData[] data;

        public Vector3Int volume_size;

        /// <summary>
        ///     The cubes which are integrated by four volumes.
        /// </summary>
        public Vector3Int cube_size;

        public VolumeMatrix(Vector3Int volumeSize)
        {
            volume_size = volumeSize;
            cube_size = new Vector3Int(
                volume_size.x - 1,
                volume_size.y - 1,
                volume_size.z - 1 );
            data = new TData[volumeSize.x * volumeSize.y * volumeSize.z];
        }

        public TData this[int x, int y, int z]
        {
            get => data[x + y * volume_size.x + z * volume_size.y * volume_size.x];
            set => data[x + y * volume_size.x + z * volume_size.y * volume_size.x] = value;
        }

        public TData this[Vector3Int idx]
        {
            get => data[idx.x + idx.y * volume_size.x + idx.z * volume_size.y * volume_size.x];
            set => data[idx.x + idx.y * volume_size.x + idx.z * volume_size.y * volume_size.x] = value;
        }

        public TData this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }

        public int Count => volume_size.x * volume_size.y * volume_size.z;

        public int CubeCount => (volume_size.x - 1) * (volume_size.y - 1) * (volume_size.z - 1);

        /// <summary>
        ///     The quad count of a VoxelMesh generated by this volume matrix
        /// </summary>
        public int VoxelMeshQuadCount => CubeCount * 12;
        /// <summary>
        ///     The vertex count of a VoxelMesh generated by this volume matrix
        /// </summary>
        public int VoxelMeshVertexCount => VoxelMeshQuadCount * 6;

        public Vector3Int CenterPoint => volume_size / 2;

        public int Index(int x, int y, int z)
        {
            return x + y * volume_size.x + z * volume_size.y * volume_size.x;
        }

        // public Vector3Int Position(int i)
        // {
        //     int z = i / (volume_size.x * volume_size.y);
        //     int y = i % (volume_size.x * volume_size.y) / volume_size.x;
        //     int x = i % volume_size.x;
        //
        //     return new Vector3Int( x, y, z );
        // } Slower version

        public Vector3Int Position(int i)
        {
            int z = i / (volume_size.x * volume_size.y);
            i -= z * volume_size.x * volume_size.y;
            int y = i / volume_size.x;
            i -= y * volume_size.x;
            int x = i;

            return new Vector3Int( x, y, z );
        }

        public bool IsEdge(int x, int y, int z)
        {
            return
                x * y * z *
                (volume_size
                 - new Vector3Int( x, y, z ) - Vector3Int.one).
                XYZProduct()
                == 0;
        }

        public void PositionFor(Action<int, int, int> action)
        {
            for (int z = 0; z < volume_size.z; z++)
            {
                for (int y = 0; y < volume_size.y; y++)
                {
                    for (int x = 0; x < volume_size.x; x++)
                    {
                        action( x, y, z );
                    }
                }
            }
        }
    }
}
