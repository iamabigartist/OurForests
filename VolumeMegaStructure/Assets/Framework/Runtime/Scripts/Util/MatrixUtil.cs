using Unity.Mathematics;
namespace VolumeMegaStructure.Util
{
	public static class MatrixUtil
	{
		public static int volume(this int3 i3)
		{
			return i3.x * i3.y * i3.z;
		}
	}
}