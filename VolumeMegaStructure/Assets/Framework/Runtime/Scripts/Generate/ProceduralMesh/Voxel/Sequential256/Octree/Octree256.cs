using Unity.Collections;
using Unity.Mathematics;
using VolumeMegaStructure.Util.JobSystem;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree.Octree256;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public struct Octree256 : ITree<OctTreeUnit, ushort>
	{
		public struct OctTreeUnit : ITreeUnit<ushort>
		{
			public ushort block_id;
			public ushort unit_size_level;
			public OctTreeUnit(ushort BlockID, ushort SizeLevel)
			{
				block_id = BlockID;
				unit_size_level = SizeLevel;
			}
			public int size_level() => unit_size_level;
			public bool Equals(OctTreeUnit other) =>
				block_id == other.block_id &&
				unit_size_level == other.unit_size_level;
			public void Deconstruct(out int size_level, out ushort info)
			{
				size_level = this.size_level();
				info = block_id;
			}
		}

		NativeHashMap<byte3, OctTreeUnit> dict;

	#region Interface

		public int size_level() => 8;
		public bool TryGetRegionByAnchor(in int3 anchor, out OctTreeUnit unit) => dict.TryGetValue(anchor, out unit);
		public void Add(in int3 anchor, in int size_level, in ushort info) => dict.Add(anchor, new(info, (ushort)size_level));
		public void Remove(in int3 anchor) => dict.Remove(anchor);

		public void Set(int3 anchor, OctTreeUnit unit)
		{
			//1. 找不到锚点，说明更大区域存在。
			//2. 找到锚点，大小一致，只改变。
			//3. 找到锚点，但是规模更小。
			//4. 找到锚点，但是规模更大。
		}

	#endregion

	#region Process

		// void Set_NoAnchor(int3 anchor, region_unit unit)
		// {
		// 	int3 up_parent_anchor = anchor;
		// 	int up_parent_size_level = unit.size_level;
		// 	region_unit up_parent_unit = unit;
		// 	while (up_parent_size_level != 8)
		// 	{
		// 		get_region_parent(up_parent_anchor, up_parent_size_level, out up_parent_anchor);
		// 		if (dict.TryGetValue(up_parent_anchor, out up_parent_unit)) { break; }
		// 		up_parent_size_level++;
		// 	}
		// 	if (up_parent_unit.block_id == unit.block_id) { return; }
		//
		// }
		void Set_ExactRegion(int3 anchor, OctTreeUnit unit)
		{
			dict[anchor] = unit;
		}

		// bool FindRegion(int3 anchor, region_unit expect_unit, out region_unit diff_unit)
		// {
		// 	if (dict.TryGetValue(anchor, out diff_unit)) {}
		// 	else
		// 	{
		// 		diff_unit = expect_unit;
		// 	}
		// }
		//
		// void CheckMergeRegion(int3 parent_anchor, region_unit parent_unit)
		// {
		// 	var expected_child_unit = new region_unit(parent_unit.block_id, (ushort)(parent_unit.size_level - 1));
		// 	for (int i = 0; i < 8; i++)
		// 	{
		// 		get_region_child(parent_anchor, parent_unit.size_level, i, out var cur_child_anchor);
		// 		if (dict.TryGetValue(cur_child_anchor, out var cur_child_unit))
		// 		{
		// 			if (cur_child_unit == expected_child_unit) {}
		// 		}
		// 	}
		// }

		void MergeRegion() {}

		void MergeTree(int3 start_anchor) {}

	#endregion

	#region Init

		public Octree256(Allocator allocator, ushort init_block_id)
		{
			dict = new(1, allocator);
			dict.Add(int3.zero, new(init_block_id, 8));
		}

	#endregion

	}
}