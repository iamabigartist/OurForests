using System;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public struct ChunkToOctree<TMatrix, TTree, TTreeUnit, TUnitInfo>
		where TMatrix : IMatrix<TUnitInfo>
		where TTree : ITree<TTreeUnit, TUnitInfo>
		where TTreeUnit : ITreeUnit<TUnitInfo>
		where TUnitInfo : struct, IEquatable<TUnitInfo>
	{
		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public static void Merge(in TMatrix matrix, ref TTree tree)
		// {
		// 	Merge(matrix, ref tree, int3.zero, matrix.size_level(), out _);
		// }

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// static bool Merge(
		// 	in TMatrix matrix,
		// 	ref TTree tree,
		// 	in int3 cur_anchor,
		// 	int expect_size_level,
		// 	out TUnitInfo info)
		// {
		// 	matrix.Get(cur_anchor, out var cur_info);
		// 	var cur_size_level = 0;
		// 	var cur_parent_region = new OctRegion(cur_anchor, cur_size_level).parent();
		// 	bool cur_can_merge = true;
		// 	while (cur_size_level != expect_size_level)
		// 	{
		// 		for (int octant = 1; octant < 8; octant++)
		// 		{
		// 			bool sibling_merged = Merge(matrix, ref tree,
		// 				cur_parent_region.child(octant).anchor, cur_size_level,
		// 				out var sibling_info);
		// 			cur_can_merge &= sibling_merged && cur_info.Equals(sibling_info);
		// 		}
		// 		cur_size_level = cur_parent_region.size_level;
		// 		cur_parent_region = cur_parent_region.parent();
		// 		if (cur_can_merge)
		// 		{
		// 			tree.Add(cur_anchor, cur_parent_region.size_level - 1, cur_info);
		// 		}
		// 	}
		// 	info = cur_info;
		// 	return cur_can_merge;
		// }

	}
}