using Unity.Collections;
namespace VolumeMegaStructure.Manage
{
	public class BlockManager
	{
		public NativeArray<int> block_group_by_id = new(new[]
		{
			0,
			1, 1, 1, 1
		}, Allocator.Persistent);
		public void Dispose()
		{
			block_group_by_id.Dispose();
		}
	}
}