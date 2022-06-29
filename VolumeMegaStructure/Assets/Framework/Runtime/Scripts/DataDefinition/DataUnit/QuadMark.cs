using System;
namespace VolumeMegaStructure.DataDefinition.DataUnit
{
	public struct QuadMark : IEquatable<QuadMark>
	{
		public int volume_i;
		public int dir;
		public QuadMark(int volume_i, int dir)
		{
			this.volume_i = volume_i;
			this.dir = dir;
		}
		public bool Equals(QuadMark other)
		{
			return volume_i.Equals(other.volume_i) && dir.Equals(other.dir);
		}
		public void Deconstruct(out int volume_i, out int dir)
		{
			volume_i = this.volume_i;
			dir = this.dir;
		}
	}
}