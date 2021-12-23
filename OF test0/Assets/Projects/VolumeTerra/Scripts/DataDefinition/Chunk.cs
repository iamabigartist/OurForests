using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MUtility;
using UnityEngine;
using UnityEngine.Rendering;
namespace VolumeTerra.DataDefinition
{
    public class Chunk
    {
        public static Mesh[] Cubes = new Mesh[1];

        public Chunk(VolumeMatrix<int> cube_matrix)
        {
            this.cube_matrix = cube_matrix;
            list_index_matrix = new VolumeMatrix<int>( cube_matrix.volume_size );
        }

        /// <summary>
        ///     Store the type of every cube in this chunk.
        /// </summary>
        public VolumeMatrix<int> cube_matrix;

        /// <summary>
        ///     Store the index of every mesh in the mesh info lists;
        /// </summary>
        VolumeMatrix<int> list_index_matrix;

    #region GeneralMesh

        FixedSegmentList<Vector3> vertices_list;
        FixedSegmentList<Vector2> uv1_list;

        /// <summary>
        ///     The final result mesh of this chunk
        /// </summary>
        public Mesh result_mesh;

        public void VolumeMatrixToSegmentLists()
        {

            var segment_length = Cubes[0].vertices.Length;
            var mesh_vertices_1 = Cubes[0].vertices;
            var mesh_uv1_1 = Cubes[0].uv;

        #region Record which position has what cube at what index in lists

            long cur_mesh_num = 0;
            var a = new ParallelOptions();
            var result = Parallel.For( 0, cube_matrix.Count,
                (i) =>
                {
                    (int x, int y, int z) = cube_matrix.Position( i );
                    int value = cube_matrix[x, y, z];

                    //Check whether empty or inside
                    if (cube_matrix.IsEdge( x, y, z ) ||
                        value == 0 ||
                        cube_matrix[x + 1, y, z] != 0 &&
                        cube_matrix[x - 1, y, z] != 0 &&
                        cube_matrix[x, y + 1, z] != 0 &&
                        cube_matrix[x, y - 1, z] != 0 &&
                        cube_matrix[x, y, z + 1] != 0 &&
                        cube_matrix[x, y, z - 1] != 0)
                    {
                        list_index_matrix[x, y, z] = -1;
                    }
                    else
                    {
                        var cur_list_index = (int)Interlocked.Increment( ref cur_mesh_num ) - 1;
                        list_index_matrix[x, y, z] = cur_list_index;
                    }

                } );

        #endregion

        #region Build the mesh info lists according to the records

            var vertices_array = new Vector3[cur_mesh_num * segment_length];
            var uv1_array = new Vector2[cur_mesh_num * segment_length];
            Parallel.For( 0, cube_matrix.Count, i =>
            {
                int list_index = list_index_matrix[i];
                if (list_index == -1) { return; }
                int start_array_index = list_index * segment_length;
                mesh_vertices_1.CopyTo( vertices_array, start_array_index );
                mesh_uv1_1.CopyTo( uv1_array, start_array_index );

                //Add local position to mesh vertices
                (int x, int y, int z) = cube_matrix.Position( i );
                for (int j = start_array_index; j < start_array_index + segment_length; j++)
                {
                    vertices_array[j] += new Vector3( x, y, z );
                }

            } );
            vertices_list = new FixedSegmentList<Vector3>( segment_length, vertices_array );
            uv1_list = new FixedSegmentList<Vector2>( segment_length, uv1_array );

        #endregion
        }

        #endregion


        public void SegmentListToResultMesh()
        {
            result_mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = vertices_list.ToArray(),
                uv = uv1_list.ToArray(),
                triangles = Enumerable.Range( 0, vertices_list.Count ).ToArray()

            };
            result_mesh.RecalculateBounds();
            result_mesh.RecalculateNormals();
            result_mesh.RecalculateTangents();
        }

    }
}
