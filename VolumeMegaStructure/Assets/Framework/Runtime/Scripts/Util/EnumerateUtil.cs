using System;
using System.Linq;
using Unity.Collections;
namespace VolumeMegaStructure.Util
{
    public static class EnumerateUtil
    {
        public static int[] ToArray(this Range r)
        {
            return Enumerable.Range( r.Start.Value, r.End.Value - r.Start.Value ).ToArray();
        }

        // public static T in_3d<T>(this NativeArray<T> array) where T : struct
        // {
        //     return array[0];
        // }
    }
}
