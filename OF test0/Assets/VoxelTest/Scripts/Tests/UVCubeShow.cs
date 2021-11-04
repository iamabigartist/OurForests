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

        (int3 c1, int axis, int dir)[] quad_iteration =
        {
            (new int3( 1, 0, 0 ), 0, 0),
            (new int3( 0, 0, 0 ), 0, 1),

            (new int3( 0, 1, 0 ), 1, 0),
            (new int3( 0, 0, 0 ), 1, 1),

            (new int3( 0, 0, 1 ), 2, 0),
            (new int3( 0, 0, 0 ), 2, 1)
        };



        void GenerateUVCube()
        {
            var m_triangle = Enumerable.Range( 0, 6 * 6 ).ToArray();
            var vertices = new int3[6 * 6];
            for (int i = 0; i < 6; i++)
            {
                (int3 c1, int axis, int dir) = quad_iteration[i];
                int3[] quad_corners =
                    VoxelGenerationUtility.corner_index_offset_in_quad[axis][dir];
                var quad_maker = new VoxelGenerationUtility.QuadMaker(
                    quad_corners[0] + c1,
                    quad_corners[1] + c1,
                    quad_corners[2] + c1,
                    quad_corners[3] + c1
                );
                quad_maker.ToVertices().CopyTo( vertices, 6 * i );
            }

        }

    }
}
