using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.Sequential_256.Octree
{
	public static class OctreeEnumerateUtil
	{
		public static int ray(in int b, int stride_log2)
		{
			return b & (1 << stride_log2);
		}
		public static int octant(in byte4 position, int stride_log2)
		{
			var bit_0 = ray(position.x, stride_log2);
			var bit_1 = ray(position.y, stride_log2);
			var bit_2 = ray(position.z, stride_log2);
			return bit_0 | bit_1 | bit_2;
		}
	}
}