using System;
using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
namespace VolumeMegaStructure.Util
{
    public static class VectorUtil
    {
        public static TResult[] select<TSource, TResult>(this TSource[] source_array, Func<TSource, TResult> func)
        {
            var result_array = new TResult[source_array.Length];

            for (int i = 0; i < result_array.Length; i++)
            {
                result_array[i] = func( source_array[i] );
            }

            return result_array;
        }
        public static float3[] floor(this float3[] array)
        {
            return array.@select( v => math.floor( v ) );
        }

        public static float3[] round(this float3[] array)
        {
            return array.@select( v => math.round( v ) );
        }

        public static Vector3[] f3_2_v3(this float3[] array)
        {
            return array.@select( v => v.ToVector() );
        }

    }
}
