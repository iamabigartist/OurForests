using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public static class TreeUtil<TTree, TTreeUnit, TUnitInfo>
		where TTree : ITree<TTreeUnit, TUnitInfo>
		where TTreeUnit : ITreeUnit<TUnitInfo>
		where TUnitInfo : struct, IEquatable<TUnitInfo>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void RegionConsistingPosition(
			in TTree tree,
			in int3 pos,
			out int3 anchor, out int size_level, out TUnitInfo info)
		{
			var region = new OctRegion(pos, 0);
			while (true)
			{
				if (tree.TryGetRegionByAnchor(region.anchor, out var unit))
				{
					anchor = region.anchor;
					(size_level, info) = unit;
					break;
				}
				else { region = region.parent(); }
			}
		}

		public static void Set(ref TTree tree, int3 anchor, int size_level, TUnitInfo info)
		{
			RegionConsistingPosition(tree, anchor,
				out var outter_anchor, out var outter_size_level, out var outter_info);
			//split
			if (outter_size_level > size_level)
			{
				var cur_region = new OctRegion(anchor, size_level);
				var cur_parent_region = cur_region.parent();
				while (size_level != outter_size_level)
				{
					for (int octant = 0; octant < 8; octant++)
					{
						if (octant == cur_region.octant) { continue; }
						tree.Add(cur_parent_region.child_anchor(octant), cur_region.size_level, info);
					}
					
				}
			}
			//merge
			else {}
		}
	}
}