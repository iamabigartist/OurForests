using UnityEngine;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public static class OctreeDebugUtil
	{
		public static string RegionString(OctRegion region)
		{
			return $"({1 << region.size_level},{region.octant})";
		}
		public static void RegionCube(OctRegion region, out Vector3 anchor_v, out Vector3 center, out Vector3 size)
		{
			anchor_v = region.anchor.v();
			var stride = 1 << region.size_level;
			size = Vector3.one * stride;
			center = anchor_v + size / 2f;
		}
	}
}