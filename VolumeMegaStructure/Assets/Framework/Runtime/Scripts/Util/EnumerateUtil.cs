using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
namespace VolumeMegaStructure.Util
{
	public static class EnumerateUtil
	{
		public static int[] ToArray(this Range r)
		{
			return Enumerable.Range(r.Start.Value, r.End.Value - r.Start.Value).ToArray();
		}

		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this NativeHashMap<TKey, TValue> native_map)
			where TKey : struct, IEquatable<TKey>
			where TValue : struct
		{
			return native_map.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		// public static T in_3d<T>(this NativeArray<T> array) where T : struct
		// {
		//     return array[0];
		// }
	}
}