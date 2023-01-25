using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
namespace VolumeMegaStructure.Util
{
	public static class DebugUtils
	{
		public static bool is_debugging = true;
		public static double Get_ms(this long ticks)
		{
			return ticks / 10000.0;
		}

		public static string JoinString<T>(this IEnumerable<T> Enumerable, string separator = ",")
		{
			return string.Join(separator, Enumerable);
		}

		public static void Log(string s)
		{
			if (is_debugging) { Debug.Log(s); }
		}

		public static void Log_Json(object o)
		{
			Log(JsonConvert.SerializeObject(o, new JsonSerializerSettings
			{
				Formatting = Formatting.Indented,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			}));
		}

		public static string IntToBitString(this int i)
		{
			var array_0 = new List<int>();
			var array = new BitArray(new[] { i });
			return array.Cast<bool>().Select(bit => bit ? "1" : "0").Reverse().JoinString("");
		}
	}
}