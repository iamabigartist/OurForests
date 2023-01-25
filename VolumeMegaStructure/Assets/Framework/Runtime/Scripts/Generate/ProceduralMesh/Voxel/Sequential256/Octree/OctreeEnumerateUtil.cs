using System.Runtime.CompilerServices;
using Unity.Mathematics;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public static class OctreeEnumerateUtil
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void rays_to_octant(in int ray_x, in int ray_y, in int ray_z, out int octant)
		{
			octant = (ray_z << 2) | (ray_y << 1) | ray_x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void octant_to_rays(out int ray_x, out int ray_y, out int ray_z, in int octant)
		{
			ray_x = octant & 1;
			ray_y = (octant >> 1) & 1;
			ray_z = octant >> 2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void get_region_ray(in int anchor_component, in int size_level, out int ray)
		{
			ray = (anchor_component & (1 << size_level)) >> size_level;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void get_region_octant(in int3 anchor, in int size_level, out int ray_x, out int ray_y, out int ray_z)
		{
			get_region_ray(anchor.x, size_level, out ray_x);
			get_region_ray(anchor.y, size_level, out ray_y);
			get_region_ray(anchor.z, size_level, out ray_z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void get_region_child(in int3 parent_anchor, in int parent_size_level, in int octant, out int3 child_anchor)
		{
			int child_size_level = parent_size_level - 1;
			octant_to_rays(out var ray_x, out var ray_y, out var ray_z, octant);
			child_anchor = parent_anchor;
			child_anchor.x &= ~(1 << child_size_level);
			child_anchor.y &= ~(1 << child_size_level);
			child_anchor.z &= ~(1 << child_size_level);
			child_anchor.x |= ray_x << child_size_level;
			child_anchor.y |= ray_y << child_size_level;
			child_anchor.z |= ray_z << child_size_level;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void get_region_parent(in int3 child_anchor, in int child_size_level, out int3 parent_anchor)
		{
			parent_anchor = child_anchor;
			parent_anchor.x &= ~(1 << child_size_level);
			parent_anchor.y &= ~(1 << child_size_level);
			parent_anchor.z &= ~(1 << child_size_level);
		}
	}
}