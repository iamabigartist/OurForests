namespace MUtility
{
    public static class BitUtils
    {

        public static int Slice(this int source, int start, int count)
        {
            return (source >> start) & ((1 << count) - 1);
        }
        public static int Slice(this byte source, int start, int count)
        {
            return (source >> start) & ((1 << count) - 1);
        }
    }
}
