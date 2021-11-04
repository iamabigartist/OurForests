using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
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

        public static Vector3 ToVector(this float3 f)
        {
            return new Vector3( f.x, f.y, f.z );
        }
        public static Vector2 ToVector(this int2 i)
        {
            return new Vector2( i.x, i.y );
        }
    }
    public static class EnumerableUtility
    {
        public static string ToMString<T>(this IEnumerable<T> array)
        {
            return string.Join( ",", array );
        }

        public static Vector3[] ToVectorArray(this float3[] array)
        {
            var v_array = new Vector3[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                v_array[i] = array[i].ToVector();
            }
            return v_array;
        }
        public static Vector2[] ToVectorArray(this int2[] array)
        {
            var v_array = new Vector2[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                v_array[i] = array[i].ToVector();
            }
            return v_array;
        }
    }
}
