namespace VolumeMegaStructure.Util
{
	public static class DebugUtils
	{
		public static double Get_ms(this long ticks)
		{
			return ticks / 10000.0;
		}
	}
}