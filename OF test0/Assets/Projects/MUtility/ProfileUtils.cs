using System.Diagnostics;
namespace MUtility
{
    public static class DebugUtils
    {
        public static string Get_ms(this Stopwatch stopwatch)
        {
            return $"{stopwatch.ElapsedTicks / 10000f} ms";
        }
    }
}
