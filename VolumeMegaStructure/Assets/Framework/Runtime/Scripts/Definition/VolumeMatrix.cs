using System;
using Unity.Mathematics;
namespace VolumeMegaStructure.Definition
{
    /// <summary>
    ///     A 3D matrix which represents particles align to uniform grid in a rectangular.
    /// </summary>
    /// <typeparam name="TData">It should be A Serializable Type.</typeparam>
    /// <remarks>The core data type of <see cref="VolumeMegaStructure" /></remarks>
    [Serializable]
    public class VolumeMatrix<TData>
    {
        public int3 size;

        public TData[] data;

        public VolumeMatrix(int3 size)
        {
            this.size = size;
            data = new TData[size.x * size.y * size.z];
        }

        public TData this[int x, int y, int z]
        {
            get => data[x + y * size.x + z * size.y * size.x];
            set => data[x + y * size.x + z * size.y * size.x] = value;
        }

        public TData this[int3 idx]
        {
            get => data[idx.x + idx.y * size.x + idx.z * size.y * size.x];
            set => data[idx.x + idx.y * size.x + idx.z * size.y * size.x] = value;
        }

        public TData this[int i]
        {
            get => data[i];
            set => data[i] = value;
        }

        public int Count => size.x * size.y * size.z;

        public int3 CenterPoint => size / 2;

        public int Index(int x, int y, int z)
        {
            return x + y * size.x + z * size.y * size.x;
        }

        public int3 Position(int i)
        {
            int z = i / (size.x * size.y);
            i -= z * size.x * size.y;
            int y = i / size.x;
            i -= y * size.x;
            int x = i;

            return new int3( x, y, z );
        }

        public bool IsEdge(int x, int y, int z)
        {
            return
                x == 0 || x == size.x - 1 ||
                y == 0 || y == size.y - 1 ||
                z == 0 || z == size.z - 1;
        }
    }
}
