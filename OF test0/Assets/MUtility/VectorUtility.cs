using Unity.Mathematics;
using UnityEngine;
using VoxelTest.Tests.Include;
namespace MUtility
{
    public static class VectorUtility
    {
        public static Vector4 ToVector4(this Vector3Int v, float w = default)
        {
            return new Vector4( v.x, v.y, v.z, w );
        }
        public static Vector4 ToVector4(this Vector3 v, float w = default)
        {
            return new Vector4( v.x, v.y, v.z, w );
        }

        public static Vector3 ToVector(this int3 int3)
        {
            return new Vector3( int3.x, int3.y, int3.z );
        }


        public static Vector3[] ToVectorArray(this int3[] array)
        {
            var v_array = new Vector3[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                v_array[i] = array[i].ToVector();
            }
            return v_array;
        }

        public static int3[] ToVerticesArray(this VoxelGenerationUtility.Quad[] quads)
        {
            var vertices = new int3[quads.Length * 6];
            for (int i = 0; i < quads.Length; i++)
            {

                for (int j = 0; j < 6; j++)
                {

                }
            }
            return vertices;
        }

    }
}
