namespace VolumeMegaStructure.DataDefinition.DataUnit
{
	public struct VolumeUnit
	{
		public ushort block_id;
		public VolumeUnit(ushort block_id) {
			this.block_id = block_id;
		}
	}
}