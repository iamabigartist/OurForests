namespace VolumeMegaStructure.Util.JobSystem
{
	public struct byte2
	{
		byte x;
		byte y;

		public byte2(byte x, byte y)
		{
			this.x = x;
			this.y = y;
		}
		public byte2(int x, int y)
		{
			this.x = (byte)x;
			this.y = (byte)y;
		}
	}
}