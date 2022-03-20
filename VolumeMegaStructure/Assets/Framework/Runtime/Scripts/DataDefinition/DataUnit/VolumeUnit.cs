namespace VolumeMegaStructure.DataDefinition.DataUnit
{
    public struct VolumeUnit
    {
        public int block_id;
        /// <summary>
        ///     0~23, totally 6 up, 4 forward face situations
        /// </summary>
        public short rotation_index;

        public void Deconstruct(out int block_id, out short rotation_index)
        {
            block_id = this.block_id;
            rotation_index = this.rotation_index;
        }
    }
}
