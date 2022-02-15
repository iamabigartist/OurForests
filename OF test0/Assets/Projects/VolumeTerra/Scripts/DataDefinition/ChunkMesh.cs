using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MUtility;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeTerra.Management;
using Debug = UnityEngine.Debug;
namespace VolumeTerra.DataDefinition
{
    /// <summary>
    ///     Using
    /// </summary>
    public class ChunkMesh
    {
        //TODO 把对于模型的选取放回静态类
        public static Mesh[] Cubes = new Mesh[1];

        public ChunkMesh(VolumeMatrix<int> cube_matrix)
        {
            this.cube_matrix = cube_matrix;
            general_list_index_matrix = new VolumeMatrix<int>( cube_matrix.volume_size );
        }

        /// <summary>
        ///     Store the type of every cube in this chunk.
        /// </summary>
        public VolumeMatrix<int> cube_matrix;

    #region Voxel

        public VolumeMatrix<Vector3Int> voxel_list_index_matrix;
        FixedSegmentList<Vector3> voxel_vertices_list;
        FixedSegmentList<Vector4> voxel_uv1_list;

        public void Voxel_VolumeMatrixToSegmentLists()
        {
            //只要不是目前的方块和左下后的要查看的方块都是Voxel，那么就能够记录生成面片。
            Vector3Int pos = Vector3Int.back;
            int cur_cube_id = 0;
            if (TerrainManager.cube_mesh_type_table[cur_cube_id] == TerrainManager.MeshType.Voxel) { }
        }

    #endregion

    #region GeneralMesh

        /// <summary>
        ///     Store the index of every mesh in the mesh info lists;
        /// </summary>
        VolumeMatrix<int> general_list_index_matrix;

        FixedSegmentList<Vector3> general_vertices_list;
        int[] general_triangle_list;
        FixedSegmentList<Vector4> general_uv1_list;
        FixedSegmentList<Vector3> general_normal_list;
        FixedSegmentList<Vector4> general_tangent_list;

        public void General_VolumeMatrixToSegmentLists()
        {

            var segment_length = Cubes[0].vertices.Length;
            var mesh_vertices_1 = Cubes[0].vertices;
            var mesh_uv1_1 = new List<Vector4>();
            Cubes[0].GetUVs( 0, mesh_uv1_1 );
            var mesh_normal_1 = Cubes[0].normals;
            var mesh_tangent_1 = Cubes[0].tangents;

        #region Record which position has what cube at what index in lists

            long cur_mesh_num = 0;
            var result = Parallel.For( 0, cube_matrix.Count,
                new ParallelOptions()
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                },
                (i) =>
                {
                    (int x, int y, int z) = cube_matrix.Position( i );
                    int value = cube_matrix[x, y, z];

                    //Check whether empty or inside or edge
                    //The edge will be crossed over 2 unit with other chunks,
                    //it aims to know the edge situation of one more inside cubes
                    if (cube_matrix.IsEdge( x, y, z ) ||
                        value == 0 ||
                        cube_matrix[x + 1, y, z] != 0 &&
                        cube_matrix[x - 1, y, z] != 0 &&
                        cube_matrix[x, y + 1, z] != 0 &&
                        cube_matrix[x, y - 1, z] != 0 &&
                        cube_matrix[x, y, z + 1] != 0 &&
                        cube_matrix[x, y, z - 1] != 0)
                    {
                        general_list_index_matrix[x, y, z] = -1;
                    }
                    else
                    {
                        var cur_list_index = (int)Interlocked.Increment( ref cur_mesh_num ) - 1;
                        general_list_index_matrix[x, y, z] = cur_list_index;
                    }

                } );

        #endregion

        #region Build the mesh info lists according to the records

            var vertices_array = new Vector3[cur_mesh_num * segment_length];
            var uv1_array = new Vector4[cur_mesh_num * segment_length];
            var normal_array = new Vector3[cur_mesh_num * segment_length];
            var tangent_array = new Vector4[cur_mesh_num * segment_length];
            Parallel.For( 0, cube_matrix.Count,
                new ParallelOptions()
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                },
                (i) =>
                {
                    int list_index = general_list_index_matrix[i];
                    if (list_index == -1) { return; }
                    int start_array_index = list_index * segment_length;
                    mesh_vertices_1.CopyTo( vertices_array, start_array_index );
                    mesh_uv1_1.CopyTo( uv1_array, start_array_index );
                    mesh_normal_1.CopyTo( normal_array, start_array_index );
                    mesh_tangent_1.CopyTo( tangent_array, start_array_index );

                    //Add local position to mesh vertices
                    (int x, int y, int z) = cube_matrix.Position( i );
                    for (int j = start_array_index; j < start_array_index + segment_length; j++)
                    {
                        vertices_array[j] += new Vector3( x, y, z );
                    }

                } );
            general_vertices_list = new FixedSegmentList<Vector3>( segment_length, vertices_array );
            //Here is the important assumption condition:
            //The index buffer i.e. the rendering order is a sequence :1,2,3,4,5,......
            general_triangle_list = Enumerable.Range( 0, general_vertices_list.Count ).ToArray();
            general_uv1_list = new FixedSegmentList<Vector4>( segment_length, uv1_array );
            general_normal_list = new FixedSegmentList<Vector3>( segment_length, normal_array );
            general_tangent_list = new FixedSegmentList<Vector4>( segment_length, tangent_array );

        #endregion
        }

    #endregion



        /// <summary>
        ///     The final result mesh of this chunk
        /// </summary>
        public Mesh result_mesh;

        public void GenerateResultMesh()
        {

            var stop = new Stopwatch();

            stop.Restart();
            result_mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32
            };
            result_mesh.SetVertices( general_vertices_list );
            result_mesh.SetTriangles( general_triangle_list, 0 );
            result_mesh.SetUVs( 0, general_uv1_list );
            result_mesh.SetNormals( general_normal_list );
            result_mesh.SetTangents( general_tangent_list );
            stop.Stop();
            Debug.Log( $"Build mesh: {stop.Get_ms()}" );

            //Concat all the parts of this chunk mesh
            result_mesh.RecalculateBounds();
            // result_mesh.RecalculateNormals();
            // result_mesh.RecalculateTangents();
        }

    }
}
