namespace VolumeMegaStructure.DataDefinition.DataUnit
{
	public struct VoxelBlockTexturePack
	{
		int right_id;
		int left_id;
		int up_id;
		int down_id;
		int forward_id;
		int back_id;
		public VoxelBlockTexturePack(int right_id, int left_id, int up_id, int down_id, int forward_id, int back_id)
		{
			this.right_id = right_id;
			this.left_id = left_id;
			this.up_id = up_id;
			this.down_id = down_id;
			this.forward_id = forward_id;
			this.back_id = back_id;
		}
	}
}