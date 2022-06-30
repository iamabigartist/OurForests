using System;
using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
namespace VolumeMegaStructure.Util
{
	public static class VectorUtil
	{

	#region Process

		///<remarks>PURPOSE More efficient than LINQ//</remarks>
		public static TResult[] select<TSource, TResult>(this TSource[] source_array, Func<TSource, TResult> func)
		{
			var result_array = new TResult[source_array.Length];

			for (int i = 0; i < result_array.Length; i++)
			{
				result_array[i] = func(source_array[i]);
			}

			return result_array;
		}

		public static float3[] floor(this float3[] array)
		{
			return array.@select(v => math.floor(v));
		}

		public static float3[] round(this float3[] array)
		{
			return array.@select(v => math.round(v));
		}

		public static Vector3[] f3_2_v3(this float3[] array)
		{
			return array.@select(v => v.ToVector());
		}

		public static Vector3 V(this int3 i3)
		{
			return new(i3.x, i3.y, i3.z);
		}

		public static Vector3 V(this float3 i3)
		{
			return new(i3.x, i3.y, i3.z);
		}

	#endregion

	#region EaseDefine

		public static readonly int3 int3_one = new(1, 1, 1);

		public static void Deconstruct(this int3 v, out int x, out int y, out int z)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}
		public static void Deconstruct(this float3 v, out float x, out float y, out float z)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}

	#endregion

	}
}