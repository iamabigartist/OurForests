using System;
using System.Linq;
namespace VolumeMegaStructure.Util
{
    public static class EnumerateUtil
    {
        public static int[] ToArray(this Range r)
        {
            return Enumerable.Range( r.Start.Value, r.End.Value - r.Start.Value ).ToArray();
        }
    }
}
