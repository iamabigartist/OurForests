using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree.OctreeEnumerateUtil;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public static class OctreeDebugUtil
	{
		public static string RegionString(int3 anchor, int size_level)
		{
			get_region_octant(anchor, size_level, out var ray_x, out var ray_y, out var ray_z);
			rays_to_octant(ray_x, ray_y, ray_z, out var octant);
			return $"({anchor.z},{anchor.y},{anchor.x}),{1 << size_level},{octant}";
		}
		public static void RegionCube(int3 anchor, int size_level, out Vector3 anchor_v, out Vector3 center, out Vector3 size)
		{
			anchor_v = anchor.v();
			var stride = 1 << size_level;
			size = Vector3.one * stride;
			center = anchor_v + size / 2f;
		}
	}
}