using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
    public class GenVoxelSourceTables
    {

    #region Data&Process

    #region Static

        static readonly float3[] VOXEL_RIGHT_QUAD =
        {
            (1, -1, -1).f3(),
            (1, 1, -1).f3(),
            (1, 1, 1).f3(),
            (1, -1, 1).f3()
        };

    #endregion

    #region Intermediate

        static float3[][] voxel_6_quad;



        static void VOXEL_RIGHT_QUAD_2_voxel_6_quad()
        {
            voxel_6_quad = new float3[][6];
            voxel_6_quad[0] = VOXEL_RIGHT_QUAD;
            voxel_6_quad[1] = VOXEL_RIGHT_QUAD.rotate( quaternion.RotateY( 180 ) );
            voxel_6_quad[2] = VOXEL_RIGHT_QUAD.rotate( quaternion.RotateZ( 90 ) );
            voxel_6_quad[3] = voxel_6_quad[2].rotate( quaternion.RotateZ( 180 ) );
            voxel_6_quad[4] = VOXEL_RIGHT_QUAD;
            voxel_6_quad[5] = VOXEL_RIGHT_QUAD;
        }

        Mesh source_voxel;

    #endregion

    #region Results

    #endregion

    #endregion

    #region

    #endregion

    }
}
