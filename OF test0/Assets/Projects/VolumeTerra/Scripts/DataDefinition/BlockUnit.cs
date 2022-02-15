namespace VolumeTerra.DataDefinition
{
    public struct BlockUnit
    {
        public uint block_index;
        /// <summary>
        ///     0~5, totally 6 up face situations
        /// </summary>
        public short up_face_index;
        /// <summary>
        ///     0~3, totally 4 forward face situations
        /// </summary>
        public short forward_face_index;
    }
}
