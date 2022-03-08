using System.Linq;
using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static Unity.Mathematics.math;
using static VolumeMegaStructure.Util.VoxelGenerationUtility;
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

        static Mesh source_voxel;

        static void init_source_voxel()
        {
            source_voxel = new();
            source_voxel.SetVertices( voxel_6_quad.SelectMany( v => v ).ToArray().f3_2_v3() );
            source_voxel.SetIndices( GenQuadIndices( 6 ), MeshTopology.Triangles, 0 );
            source_voxel.RecalculateNormals();
            source_voxel.RecalculateTangents();
        }

    #endregion

    #region Results

       public static float3[] quad_normals;
       public static float4[] quad_tangents;

        static void source_voxel_2_normal_tangent()
        {
            quad_normals = source_voxel.normals.@select( v => (float3)v );
            quad_tangents = source_voxel.tangents.@select( v => (float4)v );
        }

        public static float3[] i_rotation_i_face_i_vertex_quads;
        static void voxel_6_quad_2_i_rotation_i_face_i_vertex_quads()
        {

        }

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
