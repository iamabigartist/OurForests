using UnityEngine;
namespace VolumeTerra.Management
{
    /// <summary>
    ///     Try to implement a totally responsive system static class for the terrain assets and management.
    /// </summary>
    public class TerrainManager
    {
        /// <summary>
        ///     Different mesh type terrain will be generated separately.
        /// </summary>
        public enum MeshType
        {
            General,
            Voxel,
            MarchingCube
        }

        public TerrainManager() { }

        public static BlockConfig[] cube_configs_list;
        public static MeshType[] cube_mesh_type_table;
        public static int[] cube_transparent_group_table;
        public static Mesh[] cube_general_mesh_table;
        public static Texture2D[] texture_id_table;

    }
}
