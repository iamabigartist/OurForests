using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using VoxelTest.Tests.Include;
namespace VoxelTest.Tests
{
    public class UVCubeShow : MonoBehaviour
    {
        MeshFilter mesh_filter;
        MeshRenderer mesh_renderer;
        void Start()
        {
            mesh_filter = gameObject.AddComponent<MeshFilter>();
            mesh_renderer = gameObject.AddComponent<MeshRenderer>();
            GenerateUVCube();
        }

        void GenerateUVCube()
        {
            var m_triangle = Enumerable.Range( 0, 6 * 6 ).ToArray();
            for (int i = 0; i < 6; i++)
            {
                int3[] quad_corners = VoxelGenerationUtility.corner_index_offset_in_quad[0][0];
            }
        }

    }
}
