using System.Runtime.CompilerServices;
using Unity.Mathematics;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public struct OctRegion
	{
		public int3 anchor;
		public int size_level;
		public int octant;
		public OctRegion(int3 anchor, int size_level, int octant)
		{
			this.anchor = anchor;
			this.size_level = size_level;
			this.octant = octant;
		}

		public OctRegion(int3 anchor, int size_level)
		{
			this.anchor = anchor;
			this.size_level = size_level;
			gen_octant(anchor, size_level, out octant);
		}

	#region EaseUtils

		public void Deconstruct(out int3 _anchor, out int _size_level)
		{
			_anchor = anchor;
			_size_level = size_level;
		}

	#endregion

	#region Octant

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
		public static void gen_ray(in int anchor_component, in int size_level, out int ray)
		{
			ray = (anchor_component & (1 << size_level)) >> size_level;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void gen_rays(int3 anchor, int size_level, out int ray_x, out int ray_y, out int ray_z)
		{
			gen_ray(anchor.x, size_level, out ray_x);
			gen_ray(anchor.y, size_level, out ray_y);
			gen_ray(anchor.z, size_level, out ray_z);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void gen_octant(int3 anchor, int size_level, out int octant)
		{
			gen_rays(anchor, size_level, out var ray_x, out var ray_y, out var ray_z);
			rays_to_octant(ray_x, ray_y, ray_z, out octant);
		}

	#endregion

	#region Properties

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int3 child_anchor(in int child_octant)
		{
			int child_size_level = size_level - 1;
			octant_to_rays(out var ray_x, out var ray_y, out var ray_z, child_octant);
			int3 child_anchor = anchor;
			child_anchor.x |= ray_x << child_size_level;
			child_anchor.y |= ray_y << child_size_level;
			child_anchor.z |= ray_z << child_size_level;
			return child_anchor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly int3 parent_anchor()
		{
			int3 parent_anchor = anchor;
			parent_anchor.x &= ~(1 << size_level);
			parent_anchor.y &= ~(1 << size_level);
			parent_anchor.z &= ~(1 << size_level);
			return parent_anchor;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OctRegion child(in int child_octant)
		{
			int child_size_level = size_level - 1;
			octant_to_rays(out var ray_x, out var ray_y, out var ray_z, child_octant);
			int3 child_anchor = anchor;
			child_anchor.x |= ray_x << child_size_level;
			child_anchor.y |= ray_y << child_size_level;
			child_anchor.z |= ray_z << child_size_level;
			return new(child_anchor, child_size_level, child_octant);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OctRegion parent()
		{
			int3 parent_anchor = anchor;
			parent_anchor.x &= ~(1 << size_level);
			parent_anchor.y &= ~(1 << size_level);
			parent_anchor.z &= ~(1 << size_level);
			return new(parent_anchor, size_level + 1);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly OctRegion sibling(in int sibling_octant)
		{
			octant_to_rays(out var ray_x, out var ray_y, out var ray_z, sibling_octant);
			int3 sibling_anchor = anchor;
			sibling_anchor.x &= ~(1 << size_level);
			sibling_anchor.y &= ~(1 << size_level);
			sibling_anchor.z &= ~(1 << size_level);
			sibling_anchor.x |= ray_x << size_level;
			sibling_anchor.y |= ray_y << size_level;
			sibling_anchor.z |= ray_z << size_level;
			return new(sibling_anchor, size_level, sibling_octant);
		}

	#endregion

	#region Traverse

		public static OctRegion NextRegion(ref OctRegion parent_region, int cur_child_octant)
		{
			while (cur_child_octant != 7)
			{
				cur_child_octant = parent_region.octant;
				parent_region = parent_region.parent();
			}
			return parent_region.child(cur_child_octant + 1);
		}

	#endregion
	}
}