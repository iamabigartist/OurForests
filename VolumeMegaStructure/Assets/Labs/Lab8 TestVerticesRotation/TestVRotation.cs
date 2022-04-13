using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
namespace Labs.Lab8_TestVerticesRotation
{
    public class TestVRotation : MonoBehaviour
    {
        static readonly int2[] uv_list =
        {
            (0, 0).i2(),
            (1, 0).i2(),
            (1, 1).i2(),
            (0, 1).i2()
        };

        /// <summary>
        ///     Construct a quad with 4 vertices, each with a uv in the const list.
        /// </summary>
        static Mesh InitQuad()
        {
            var mesh = new Mesh
            {
                vertices = GenVoxelSourceTables.i_rotation_i_face_i_vertex_quads[].ToVectorArray(),
                uv = uv_list.ToVectorArray(),
                triangles = new[] { 0, 1, 2, 2, 3, 0 }
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        Transform transform_mesh;
        Transform table_mesh;
        MeshFilter table_mesh_filter;

        void Start()
        {
            transform_mesh = transform.Find( "TransformQuad" );
            transform_mesh.GetComponent<MeshFilter>().mesh = InitQuad();
        }

        void Update() { }
    }
}
