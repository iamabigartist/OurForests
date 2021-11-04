using System.Linq;
using MUtility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VoxelTest.Tests.Include;
namespace VoxelTest.Tests
{
    public class UVCubeShow : MonoBehaviour
    {
        MeshFilter mesh_filter;
        MeshRenderer mesh_renderer;
        Mesh m_mesh;
        void Start()
        {
            mesh_filter = gameObject.GetComponent<MeshFilter>();
            mesh_renderer = gameObject.GetComponent<MeshRenderer>();
            m_mesh = new Mesh()
            {
                indexFormat = IndexFormat.UInt32
            };
            GenerateUVCube();

            mesh_filter.sharedMesh = m_mesh;
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
            const int quad_count = 6;
            const int vertices_count = quad_count * 6;
            var triangles = Enumerable.Range( 0, vertices_count ).ToArray();
            var vertices = new float3[vertices_count];
            var uv = new int2[vertices_count];
            var uv2 = new int2[vertices_count];
            for (int i = 0; i < quad_count; i++)
            {
                (int3 c1, int axis, int dir) = quad_iteration[i];

                //Generate vertices
                int3[] quad_corner_offsets =
                    VoxelGenerationUtility.corner_index_offset_in_quad[axis][dir];
                var quad_maker = new VoxelGenerationUtility.QuadMaker(
                    quad_corner_offsets[0] + c1,
                    quad_corner_offsets[1] + c1,
                    quad_corner_offsets[2] + c1,
                    quad_corner_offsets[3] + c1
                );
                quad_maker.ToVertices().CopyTo( vertices, 6 * i );

                //Generate uv0: position
                var quad_corner_uv_indices =
                    VoxelGenerationUtility.corner_uv_index_in_quad[axis][dir];
                var quad_corner_uv =
                    VoxelGenerationUtility.triangle_order_in_quad.Select(
                        (index) =>
                            VoxelGenerationUtility.uv_4p
                                [quad_corner_uv_indices[index]] ).ToArray();
                quad_corner_uv.CopyTo( uv, 6 * i );

                //Generate uv1:texture index
                var quad_texture_index =
                    Enumerable.Repeat( new int2( i, 0 ), 6 ).ToArray();
                quad_texture_index.CopyTo( uv2, 6 * i );
            }

            print( vertices.ToMString() );
            print( uv.ToMString() );
            print( uv2.ToMString() );
            m_mesh.vertices = vertices.ToVectorArray();
            m_mesh.triangles = triangles;
            m_mesh.uv = uv.ToVectorArray();
            m_mesh.uv2 = uv2.ToVectorArray();
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
        }

    }
}
