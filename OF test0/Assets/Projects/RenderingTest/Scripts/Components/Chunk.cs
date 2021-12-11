using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VolumeTerra.DataDefinition;
namespace RenderingTest.Components
{
    public class Chunk
    {
        public static void InitMeshLists1(
            out List<Vector3[]> vertices_arrays_list,
            out List<Vector2[]> uv1_arrays_list,
            out List<int[]> triangles_arrays_list,
            out List<int> vertices_array_starts,
            out List<int> triangles_array_starts,
            VolumeMatrix<int> matrix,
            Dictionary<int, Mesh> mesh_dict)
        {
            vertices_arrays_list = new List<Vector3[]>();
            uv1_arrays_list = new List<Vector2[]>();
            triangles_arrays_list = new List<int[]>();
            vertices_array_starts = new List<int>();
            triangles_array_starts = new List<int>();

            int cur_vertex_count = 0;
            int cur_triangle_count = 0;
            for (int z = 0; z < matrix.volume_size.z; z++)
            {
                for (int y = 0; y < matrix.volume_size.y; y++)
                {
                    for (int x = 0; x < matrix.volume_size.x; x++)
                    {
                        int value = matrix[x, y, z];

                        //Check whether empty or inside
                        if (value == 0 ||
                            matrix[x + 1, y, z] != 0 &&
                            matrix[x - 1, y, z] != 0 &&
                            matrix[x, y + 1, z] != 0 &&
                            matrix[x, y - 1, z] != 0 &&
                            matrix[x, y, z + 1] != 0 &&
                            matrix[x, y, z - 1] != 0) { continue; }

                        var vertices = mesh_dict[value].vertices;
                        var uv1 = mesh_dict[value].uv;
                        var triangles = mesh_dict[value].triangles;

                        vertices_arrays_list.Add( vertices );
                        uv1_arrays_list.Add( uv1 );
                        triangles_arrays_list.Add( triangles );
                        vertices_array_starts.Add( cur_vertex_count );
                        triangles_array_starts.Add( cur_triangle_count );
                        cur_vertex_count += vertices.Length;
                        cur_triangle_count += triangles.Length;
                    }
                }
            }

        }

        public static void InitMeshInfos(
            out List<Vector3[]> vertices_arrays_list,
            out List<Vector2[]> uv1_arrays_list,
            out List<int[]> triangles_arrays_list,
            out List<int> vertices_array_starts,
            out List<int> triangles_array_starts,
            out Dictionary<Vector3Int, int> meshes_dict,
            VolumeMatrix<int> matrix,
            Dictionary<int, Mesh> mesh_dict)
        {
            vertices_arrays_list = new List<Vector3[]>();
            uv1_arrays_list = new List<Vector2[]>();
            triangles_arrays_list = new List<int[]>();
            vertices_array_starts = new List<int>();
            triangles_array_starts = new List<int>();

            int cur_vertex_count = 0;
            int cur_triangle_count = 0;

            meshes_dict = new Dictionary<Vector3Int, int>();
            int cur_mesh_count = 0;
            //主要后续想法就是说用一个和矩阵一样长容量的列表去存储，首先是判断模型信息，包括是否有模型，模型位置，模型值；然后把模型信息加入到列表里面，使用lock来操作。然后再将列表的对应下表与模型位置关系加入到字典里面。这样完成了预处理。
            Parallel.For( 0, matrix.Count, (i, state) =>
            {
                Vector3Int position = matrix.Position( i );

            } );

            //然后下面多线程实际加入模型操作的时候，遍历模型信息列表，新建模型对象然后拷贝每个模型，到最终的列表里面。并且记录局部加和长度
            //由于triangle 给的是index是一个局部信息，并且不能通过shader重复计算，每次中间模型被更改的时候都要给每个triangle元素加上修改偏移量，非常麻烦。然后如果使用全顶点模型，体积会加大到1.5倍，但是实际上模型本身就不大，然后在渲染上来说，渲染速度还是取决与实际有多少三角形而非有多少顶点，因此预计不会带来性能问题。所以后面打算放弃使用三角形，而是把看到的所有模型转换成纯顶点模型。
            for (int z = 0; z < matrix.volume_size.z; z++)
            {
                for (int y = 0; y < matrix.volume_size.y; y++)
                {
                    for (int x = 0; x < matrix.volume_size.x; x++)
                    {
                        int value = matrix[x, y, z];

                        //Check whether empty or inside
                        if (value == 0 ||
                            matrix[x + 1, y, z] != 0 &&
                            matrix[x - 1, y, z] != 0 &&
                            matrix[x, y + 1, z] != 0 &&
                            matrix[x, y - 1, z] != 0 &&
                            matrix[x, y, z + 1] != 0 &&
                            matrix[x, y, z - 1] != 0) { continue; }

                        var vertices = mesh_dict[value].vertices;
                        var uv1 = mesh_dict[value].uv;
                        var triangles = mesh_dict[value].triangles;

                        vertices_arrays_list.Add( vertices );
                        uv1_arrays_list.Add( uv1 );
                        triangles_arrays_list.Add( triangles );
                        vertices_array_starts.Add( cur_vertex_count );
                        triangles_array_starts.Add( cur_triangle_count );
                        cur_vertex_count += vertices.Length;
                        cur_triangle_count += triangles.Length;
                    }
                }
            }

        }
    }
}
