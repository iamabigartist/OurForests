using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
    public class GenVoxelSourceTables
    {

    #region Data&Process

    #region Static

        static readonly float3[] VOXEL_RIGHT_QUAD =
        {
            (1f, -1f, -1f).V(),
            (1f, 1f, -1f).V(),
            (1f, 1f, 1f).V(),
            (1f, -1f, 1f).V()
        };

        static readonly float3[,] VOXEL_6_QUAD;

        static void VOXEL_RIGHT_QUAD_2_VOXEL_6_QUAD() { }

    #endregion

    #region Intermediate

        Mesh source_voxel;

    #endregion

    #region Results

    #endregion

    #endregion

    #region

    #endregion

    }
}
