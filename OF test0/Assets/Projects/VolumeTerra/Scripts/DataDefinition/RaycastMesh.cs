using System.Linq;
using MUtility;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeTerra.Generate.SourceGenerator;
namespace VolumeTerra.DataDefinition
{
    public class RaycastMesh
    {
        Camera m_camera;
        VoxelSurfaceUVOld m_surfaceUV;
        /// <summary>
        ///     Store the type of every cube in this chunk.
        /// </summary>
        public VolumeMatrix<int> cube_matrix;
        FixedSegmentList<Vector3> voxel_vertices_list;
        FixedSegmentList<Vector2> voxel_uv1_list;
        /// <summary>
        ///     The final result mesh of this chunk
        /// </summary>
        public Mesh result_mesh;



        public void GenerateResultMesh()
        {

            //Concat all the parts of this chunk mesh
            result_mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = voxel_vertices_list.ToArray(),
                uv = voxel_uv1_list.ToArray(),
                triangles = Enumerable.Range( 0, voxel_vertices_list.Count ).ToArray()

            };
            result_mesh.RecalculateBounds();
            result_mesh.RecalculateNormals();
            result_mesh.RecalculateTangents();
        }
    }
}
