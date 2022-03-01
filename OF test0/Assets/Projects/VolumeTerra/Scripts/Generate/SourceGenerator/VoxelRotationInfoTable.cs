using System;
using System.Linq;
using MUtility;
using UnityEngine;
using static MUtility.VoxelGenerationUtility;
namespace VolumeTerra.Generate.SourceGenerator
{
    public class VoxelRotationInfoTable
    {
    #region Reference

        VoxelSourceMesh voxel_source_mesh;

    #endregion

    #region Util

        //As the 0,0,0 is not the center of the cube, it needs an offset to rotate the cube around the 0,0,0 point.
        public static readonly Vector3 offset = new(-0.5f, -0.5f, -0.5f);

    #endregion

    #region Data

        int[,][] vertex_uv_indices_table;

    #endregion

    #region Process

        int[] GenerateCubeVertexUVIndices(int up_face_index, int forward_face_index)
        {
            var rotation = LookRotation( up_face_index, forward_face_index );
            var source_face_normals = index2normal_vector3d;
            var source_vertices = voxel_source_mesh.Vertices;
            var source_vertex_uv_indices = voxel_source_mesh.VertexUVIndices;
            var rotated_face_normals =
                source_face_normals.Select( n =>
                    (rotation * n).Round() ).ToArray();
            var rotated_vertices =
                source_vertices.Select( v =>
                        (rotation * v).Round() ).
                    ToArray(); //36 vertex
            var new_vertex_uv_indices = Enumerable.Repeat( -1, 36 ).ToArray();

            for (int face_i = 0; face_i < 6; face_i++)
            {
                bool up_tri = true;
                var new_face_i = Array.IndexOf( source_face_normals, rotated_face_normals[face_i] );

                for (int local_vertex_i = 0; local_vertex_i < 6; local_vertex_i++)
                {
                    var vertex_i = face_i * 6 + local_vertex_i;
                    var new_local_vertex_i_2 = source_vertices[(new_face_i * 6)..(new_face_i * 6 + 6)].AllIndexOf( rotated_vertices[vertex_i] );

                    for (int j = 0; j < new_local_vertex_i_2.Length; j++)
                    {
                        var cur_new_local_vertex_i = new_local_vertex_i_2[j];

                        var new_vertex_i = new_face_i * 6 + cur_new_local_vertex_i;

                        if (new_vertex_uv_indices[new_vertex_i] != -1)
                        {
                            Debug.Log( $"rotation:{{up: {up_face_index}, forward: {forward_face_index}}},\n" +
                                       $"face_i: {face_i}, " +
                                       $"local_vertex_i: {local_vertex_i}, " +
                                       $"new_face_i: {new_face_i}, " +
                                       $"new_local_vertex_i: {cur_new_local_vertex_i}" );
                        }

                        new_vertex_uv_indices[new_vertex_i] = source_vertex_uv_indices[vertex_i];
                    }
                }
            }

            return new_vertex_uv_indices;
        }

        void GenerateVertexUVIndicesTable()
        {
            vertex_uv_indices_table = new int[6, 6][];

            for (int up_i = 0; up_i < 6; up_i++)
            {
                for (int forward_i = 0; forward_i < 6; forward_i++)
                {
                    if (ValidLookRotation( up_i, forward_i ))
                    {
                        vertex_uv_indices_table[up_i, forward_i] =
                            GenerateCubeVertexUVIndices( up_i, forward_i );
                    }
                    else
                    {
                        vertex_uv_indices_table[up_i, forward_i] = null;
                    }

                }
            }
        }

    #endregion

    #region API

        public VoxelRotationInfoTable(VoxelSourceMesh VoxelSourceMesh)
        {
            voxel_source_mesh = VoxelSourceMesh;
            GenerateVertexUVIndicesTable();
        }


        /// <summary>
        ///     Get the rotated cube vertex_uv_indices.
        /// </summary>
        /// <remarks>count = 36</remarks>
        public void GetCubeVertexUVIndices(
            int up_face_index,
            int forward_face_index,
            int[] cube_vertex_uv_indices)
        {
            Array.Copy( vertex_uv_indices_table[up_face_index, forward_face_index], cube_vertex_uv_indices, 36 );
        }

        /// <summary>
        ///     Get the rotated face vertex_uv_indices.
        /// </summary>
        /// <remarks>count = 6</remarks>
        public void GetFaceVertexUVIndices(
            int target_face_index,
            int up_face_index,
            int forward_face_index,
            int[] face_vertex_uv_indices)
        {
            var source_start_index = target_face_index * 6;
            Array.Copy(
                vertex_uv_indices_table[up_face_index, forward_face_index], source_start_index,
                face_vertex_uv_indices, 0,
                6 );
        }

    #endregion
    }
}
