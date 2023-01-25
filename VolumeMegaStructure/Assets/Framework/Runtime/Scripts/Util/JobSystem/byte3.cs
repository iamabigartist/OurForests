using System;
using Unity.Mathematics;
namespace VolumeMegaStructure.Util.JobSystem
{
	public struct byte3 : IEquatable<byte3>
	{
		public byte x;
		public byte y;
		public byte z;

		public byte3(byte x, byte y, byte z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}
		public byte3(int x, int y, int z)
		{
			this.x = (byte)x;
			this.y = (byte)y;
			this.z = (byte)z;
		}
		public bool Equals(byte3 other)
		{
			return x == other.x && y == other.y && z == other.z;
		}

		public static implicit operator int3(byte3 b3) => new(b3.x, b3.y, b3.z);
		public static implicit operator byte3(int3 i3) => new(i3.x, i3.y, i3.z);
	}
}