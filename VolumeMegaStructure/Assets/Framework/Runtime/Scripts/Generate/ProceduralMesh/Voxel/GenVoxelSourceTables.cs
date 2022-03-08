using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
    public class GenVoxelSourceTables
    {

    #region Data&Process

    #region Static

        static readonly float3[] voxel_right_quad =
        {
            (1, -1, -1).f3(),
            (1, 1, -1).f3(),
            (1, 1, 1).f3(),
            (1, -1, 1).f3()
        };

    #endregion

    #region Intermediate

        static float3[][] voxel_6_quad;

        static void voxel_right_quad_2_6_quad()
        {
            voxel_6_quad = new float3[6][];
            voxel_6_quad[0] = voxel_right_quad;
            voxel_6_quad[1] = voxel_right_quad.rotate( quaternion.RotateY( radians( 180 ) ) ).round();
            voxel_6_quad[4] = voxel_right_quad.rotate( quaternion.RotateY( radians( -90 ) ) ).round();
            voxel_6_quad[5] = voxel_right_quad.rotate( quaternion.RotateY( radians( 90 ) ) ).round();
            voxel_6_quad[2] = voxel_6_quad[4].rotate( quaternion.RotateX( radians( -90 ) ) ).round();
            voxel_6_quad[3] = voxel_6_quad[4].rotate( quaternion.RotateX( radians( 90 ) ) ).round();
        }

        Mesh source_voxel;

    #endregion

    #region Results

    #endregion

    #endregion

    #region Entry

        static GenVoxelSourceTables()
        {
            voxel_right_quad_2_6_quad();
        }

    #endregion

    }
}
