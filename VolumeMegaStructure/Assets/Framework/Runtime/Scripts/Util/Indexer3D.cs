using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
namespace VolumeMegaStructure.Util
{
    // [Obsolete( "No aim used" )]
    public struct Indexer3D
    {
        public int3 size;
        public Indexer3D(int3 size) { this.size = size; }

        /// <summary>
        ///     Get flatten index
        /// </summary>
        public int this[int x, int y, int z]
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => x + y * size.x + z * size.y * size.x;
        }

        /// <summary>
        ///     Get flatten index
        /// </summary>
        public int this[int3 i3]
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => i3.x + i3.y * size.x + i3.z * size.y * size.x;
        }

        /// <summary>
        ///     Get position from index
        /// </summary>
        public int3 this[int i]
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get
            {
                int z = i / (size.x * size.y);
                i -= z * size.x * size.y;
                int y = i / size.x;
                i -= y * size.x;
                int x = i;

                return new int3( x, y, z );
            }
        }

        public bool IsEdge(int x, int y, int z)
        {
            return
                x == 0 || x == size.x - 1 ||
                y == 0 || y == size.y - 1 ||
                z == 0 || z == size.z - 1;
        }

        public bool OutOfRange(int x, int y, int z)
        {
            return
                x < 0 || x > size.x - 1 ||
                y < 0 || y > size.y - 1 ||
                z < 0 || z > size.z - 1;
        }



    }
}
