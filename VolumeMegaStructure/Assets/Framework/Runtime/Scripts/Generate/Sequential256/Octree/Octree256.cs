using Unity.Collections;
using Unity.Mathematics;
using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.Sequential256.Octree
{
	public struct Octree256
	{
		public struct region_unit
		{
			ushort block_id;
			ushort size_level;
			public region_unit(ushort BlockID, ushort SizeLevel)
			{
				block_id = BlockID;
				size_level = SizeLevel;
			}
		}
		NativeHashMap<byte3, region_unit> dict;

	#region Process

		void Set_ExactRegion(int3 anchor, ushort block_id) {}

		void Merge(int3 start_anchor) {}

	#endregion

	#region Interface

		public void Set(int3 anchor, region_unit unit)
		{
			//1. 找不到锚点，说明更大区域存在。
			//2. 找到锚点，大小一致，只改变。
			//3. 找到锚点，但是规模更小。
			//4. 找到锚点，但是规模更大。
		}

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