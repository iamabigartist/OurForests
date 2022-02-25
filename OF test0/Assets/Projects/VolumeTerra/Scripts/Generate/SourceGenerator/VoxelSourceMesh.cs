﻿using System;
using System.Linq;
using MUtility;
using Unity.Mathematics;
using UnityEngine;
using static MUtility.VoxelGenerationUtility;
namespace VolumeTerra.Generate.SourceGenerator
{

    public class VoxelSourceMesh
    {

    #region Util

        /// <summary>
        ///     The quad generation order, in the order of surface index <see cref="VoxelGenerationUtility.index2normal_vector3d" />
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
        public static readonly Vector3 offset = new(-0.5f, -0.5f, -0.5f);

        const int quad_count = 6;
        const int vertices_count_per_quad = 6;
        const int vertices_count = vertices_count_per_quad * quad_count;

    #endregion

    #region Data

        Mesh source;
        Vector3[] vertices;
        Vector2[] uvs;
        int[] vertex_uv_indices;
        Vector3[] face_normals;
        Vector4[] face_tangents;

    #endregion

    #region Process

        void Generate()
        {

        #region Gen SourceMesh and verrtex uv indices

            source = new Mesh();
            vertices = new Vector3[vertices_count];
            uvs = new Vector2[vertices_count];
            vertex_uv_indices = new int[vertices_count];
            var triangles = (..vertices_count).ToArray();



            for (int i = 0; i < quad_count; i++)
            {
                (int3 c1, int axis, int dir) = quad_iteration[i];
                int vertex_start_index = 6 * i;
                //Generate vertices
                int3[] quad_corner_offsets =
                    vertex_corner_offset_in_quad[axis][dir];

                //the 0,0,0 will be the center of the mesh if add offset to each vert
                var quad_corner_position = new float3[]
                {
                    quad_corner_offsets[0] + c1,
                    quad_corner_offsets[1] + c1,
                    quad_corner_offsets[2] + c1,
                    quad_corner_offsets[3] + c1
                };
                var quad_maker = new QuadMaker( quad_corner_position );
                quad_maker.ToVertices().ToVectorArray().CopyTo( vertices, vertex_start_index );

                //Generate vertex uv index and uv
                var quad_uv_indices_4 =
                    vertex_uv_index_in_quad[axis][dir];
                var quad_uv_indices_6 =
                    triangle_order_in_quad.Select(
                        (index) => quad_uv_indices_4[index]
                    ).ToArray();
                var quad_uvs = quad_uv_indices_6.Select( i => uv_4p[i] ).ToArray().ToVectorArray();
                quad_uv_indices_6.CopyTo( vertex_uv_indices, vertex_start_index );
                quad_uvs.CopyTo( uvs, vertex_start_index );
            }

            source.SetVertices( vertices );

            source.SetUVs( 0, uvs );

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

    #region API

        public VoxelSourceMesh()
        {
            Generate();
        }

        public void GetFaceVertices(
            int face_index,
            Vector3[] face_vertices)
        {
            var source_start_index = face_index * 6;
            Array.Copy(
                vertices, source_start_index,
                face_vertices, 0,
                vertices_count_per_quad );
        }

        public void GetSourceFace(
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

        public Vector3[] Vertices => vertices;
        public int[] VertexUVIndices => vertex_uv_indices;


        /// <summary>
        ///     Set data for the 3 compute buffer containing voxel mesh info
        /// </summary>
        /// <param name="uv_buffer">out 4 float*2</param>
        /// <param name="normal_buffer">out 6 float*3</param>
        /// <param name="tangent_buffer">out 6 float*4</param>
        /// <returns></returns>
        public void SetMeshInfoBuffers(
            ComputeBuffer uv_buffer,
            ComputeBuffer normal_buffer,
            ComputeBuffer tangent_buffer)
        {
            uv_buffer.SetData( uv_4p );
            normal_buffer.SetData( face_normals );
            tangent_buffer.SetData( face_tangents );
        }

    #endregion




    }
}