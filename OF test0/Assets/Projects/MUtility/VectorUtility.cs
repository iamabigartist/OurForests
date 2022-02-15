using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
namespace MUtility
{
    [Serializable]
    public struct Triangle
    {
        public Vector3 x;
        public Vector3 y;
        public Vector3 z;
        public Triangle(Vector3 x, Vector3 y, Vector3 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public static class VectorUtility
    {


        public static int XYZProduct(this Vector3Int v)
        {
            return v.x * v.y * v.z;
        }
        public static void Deconstruct(this Vector3Int v, out int x, out int y, out int z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public static void Deconstruct(this Vector3 v, out float x, out float y, out float z)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

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
        public static Vector2 ToVector(this float2 i)
        {
            return new Vector2( i.x, i.y );
        }
        public static Quaternion ToQuaternion(this Vector3 v)
        {
            return Quaternion.Euler( v );
        }

        public static Color ToColor(this Vector3 v)
        {
            return new Color( v.x, v.y, v.z, 0 );
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
        public static Vector2[] ToVectorArray(this float2[] array)
        {
            var v_array = new Vector2[array.Length];
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
