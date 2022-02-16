using System;
using System.Linq;
using MUtility;
using Unity.Mathematics;
using UnityEngine;
namespace VolumeTerra.Generate.SourceGenerator
{

    public class VoxelSourceMesh
    {

    #region Util

        /// <summary>
        ///     The quad generation order, in the order of surface index <see cref="VoxelGenerationUtility.index2vector3d" />
        /// </summary>
        (int3 c1, int axis, int dir)[] quad_iteration =
        {
            (new int3( 1, 0, 0 ), 0, 0),
            (new int3( 0, 0, 0 ), 0, 1),

            (new int3( 0, 1, 0 ), 1, 0),
            (new int3( 0, 0, 0 ), 1, 1),

            (new int3( 0, 0, 1 ), 2, 0),
            (new int3( 0, 0, 0 ), 2, 1)
        };

        //the 0,0,0 will be the center of the mesh if add offset to each vert
        float3 offset = new(-0.5f, -0.5f, -0.5f);

        const int quad_count = 6;
        const int vertices_count_per_quad = 6;
        const int vertices_count = vertices_count_per_quad * quad_count;

    #endregion

    #region Data

        Mesh source;
        Vector3[] vertices;
        int[] vertex_uv_indices;
        Vector3[] face_normals;
        Vector4[] face_tangents;

    #endregion

    #region Process

        void Generate()
        {

        #region Gen SourceMesh and verrtex uv indices

            source = new Mesh();
            vertex_uv_indices = new int[vertices_count];

            var triangles = Enumerable.Range( 0, vertices_count ).ToArray();
            var vertices_ = new float3[vertices_count];



            for (int i = 0; i < quad_count; i++)
            {
                (int3 c1, int axis, int dir) = quad_iteration[i];

                //Generate vertices
                int3[] quad_corner_offsets =
                    VoxelGenerationUtility.corner_index_offset_in_quad[axis][dir];

                //the 0,0,0 will be the center of the mesh if add offset to each vert
                var quad_corner_position = new float3[]
                {
                    quad_corner_offsets[0] + c1,
                    quad_corner_offsets[1] + c1,
                    quad_corner_offsets[2] + c1,
                    quad_corner_offsets[3] + c1
                };
                var quad_maker = new VoxelGenerationUtility.QuadMaker( quad_corner_position );
                quad_maker.ToVertices().CopyTo( vertices_, 6 * i );

                //Generate vertex uv index
                var quad_corner_uv_indices =
                    VoxelGenerationUtility.corner_uv_index_in_quad[axis][dir];
                var quad_corner_uv =
                    VoxelGenerationUtility.triangle_order_in_quad.Select(
                        (index) => quad_corner_uv_indices[index] //The surface index is just sequence.
                    ).ToArray();
                quad_corner_uv.CopyTo( vertex_uv_indices, 6 * i );
            }

            //for get vertices only usage
            vertices = vertices_.ToVectorArray();

            source.SetVertices( vertices_.ToVectorArray() );

            source.SetTriangles( triangles, 0 );

            source.RecalculateNormals();

            source.RecalculateTangents();

        #endregion

        #region Get Normals and Tangents

            face_normals = new Vector3[quad_count];
            face_tangents = new Vector4[quad_count];
            var vertex_normals = source.normals;
            var vertex_tangents = source.tangents;
            for (int i = 0; i < quad_count; i++)
            {
                face_normals[i] = vertex_normals[vertices_count_per_quad * i];
                face_tangents[i] = vertex_tangents[vertices_count_per_quad * i];
            }

        #endregion

        }

    #endregion

    #region Interface

        public VoxelSourceMesh()
        {
            Generate();
        }


        /// <summary>
        ///     Get the vertices positions and vertex_uv_indices of vertices on this face using the face index and rotation index.
        ///     Won't check for out of range.
        /// </summary>
        /// <param name="face_index"></param>
        /// <param name="face_vertices">out float*3</param>
        /// <param name="face_vertex_uv_indices">out int</param>
        public void GetFace(
            int face_index,
            Vector3[] face_vertices,
            int[] face_vertex_uv_indices)
        {
            var source_start_index = face_index * 6;
            Array.Copy(
                vertices, source_start_index,
                face_vertices, 0,
                vertices_count_per_quad );
            Array.Copy(
                vertex_uv_indices, source_start_index,
                face_vertex_uv_indices, 0,
                vertices_count_per_quad );
        }


        /// <summary>
        ///     Set data for the 3 compute buffer containing voxel mesh info
        /// </summary>
        /// <param name="uv_buffer">out float*2</param>
        /// <param name="normal_buffer">out float*3</param>
        /// <param name="tangent_buffer">out float*4</param>
        /// <returns></returns>
        public void GetVectorTexture(
            ComputeBuffer uv_buffer,
            ComputeBuffer normal_buffer,
            ComputeBuffer tangent_buffer)
        {
            uv_buffer.SetData( VoxelGenerationUtility.uv_4p );
            normal_buffer.SetData( face_normals );
            tangent_buffer.SetData( face_tangents );
        }

    #endregion




    }
}
