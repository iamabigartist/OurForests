using System;
namespace VolumeMegaStructure.Util.JobSystem
{
	public struct byte4 : IEquatable<byte4>
	{
		public byte x;
		public byte y;
		public byte z;

		public byte4(byte x, byte y, byte z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public byte4(int x, int y, int z)
		{
			this.x = (byte)x;
			this.y = (byte)y;
			this.z = (byte)z;
		}
		public bool Equals(byte4 other)
		{
			return x == other.x && y == other.y && z == other.z;
		}
	}
}