using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
    public struct VolumeMatrixScan : IJobParallelFor
    {

    #region Table

        [ReadOnly] public NativeArray<int> id_to_transparency_table;

    #endregion

    #region Data

        [ReadOnly] public Indexer3D indexer;
        [NativeDisableParallelForRestriction][ReadOnly] public NativeArray<VolumeUnit> volume_matrix;
        [NativeDisableParallelForRestriction][WriteOnly] public NativeArray<int> mark_matrix;
        NativeCounter quad_counter;

    #endregion

        public void Execute(int volume_i)
        {
            var (x, y, z) = indexer[volume_i];
            var (cur_block_id, cur_rotation_index) = volume_matrix[volume_i];
            var cur_transparency = id_to_transparency_table[cur_block_id];

            if (indexer.IsEdge( x, y, z ))
            {
                return;
            }

            var (x_forward_block_id, x_forward_rotation_index) = volume_matrix[indexer[x + 1, y, z]];
            var (y_forward_block_id, y_forward_rotation_index) = volume_matrix[indexer[x, y + 1, z]];
            var (z_forward_block_id, z_forward_rotation_index) = volume_matrix[indexer[x, y, z + 1]];

            var x_forward_transparency = id_to_transparency_table[x_forward_block_id];
            var y_forward_transparency = id_to_transparency_table[y_forward_block_id];
            var z_forward_transparency = id_to_transparency_table[z_forward_block_id];

            if (x_forward_transparency != cur_transparency)
            {
                
            }

            if (y_forward_transparency != cur_transparency) { }

            if (z_forward_transparency != cur_transparency) { }


        }

    }
}
