using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;
using static MUtility.VoxelGenerationUtility;
namespace VolumeTerra.Object
{

    public struct SurfaceNormalDirection
    {

        /// <summary>
        ///     x->0,y->1,z->2
        /// </summary>
        public int axis;

        /// <summary>
        ///     + -> 0, - -> 1.
        /// </summary>
        public int positive;


        public SurfaceNormalDirection(int axis, int positive)
        {
            this.axis = axis;
            this.positive = positive;
        }
    }


    public class VoxelSurfaceUV
    {

        /// <summary>
        ///     Rotate the original direction once
        /// </summary>
        /// <remarks>
        ///     return the new index. [original_index][rotate_axis][clockwise/counterclockwise]
        /// </remarks>
        static int[][][] rotate_once =
        {
            //0
            new[]
            {
                new[] { 0, 0 },
                new[] { 5, 4 },
                new[] { 2, 3 }

            },
            //1
            new[]
            {
                new[] { 1, 1 },
                new[] { 4, 5 },
                new[] { 3, 2 }
            },
            //2
            new[]
            {
                new[] { 4, 5 },
                new[] { 2, 2 },
                new[] { 1, 0 }
            },
            //3
            new[]
            {
                new[] { 5, 4 },
                new[] { 3, 3 },
                new[] { 0, 1 }
            },
            //4
            new[]
            {
                new[] { 3, 2 },
                new[] { 0, 1 },
                new[] { 4, 4 }
            },
            //5
            new[]
            {
                new[] { 2, 3 },
                new[] { 1, 0 },
                new[] { 5, 5 }
            }
        };



        public static SurfaceNormalDirection IndexToNormal(int index)
        {
            var value = index2normal[index];
            return new SurfaceNormalDirection( value[0], value[1] );
        }

        public static int NormalToIndex(SurfaceNormalDirection direction)
        {
            return normal2index[direction.axis][direction.positive];
        }

        public static Vector3 NormalDirectionToVector3(SurfaceNormalDirection direction)
        {
            return index2vector3d[NormalToIndex( direction )];
        }

        /// <summary>
        /// </summary>
        /// <param name="ori_index">The index of the original direction </param>
        /// <param name="rotate_axis">x,y,z</param>
        /// <param name="rotate_times">positive for clockwise, negative for counterclockwise</param>
        /// <returns>The direction index after rotation </returns>
        public static int RotateSurface(int ori_index, int rotate_axis, int rotate_times)
        {
            if (rotate_times == 0)
            {
                return ori_index;
            }
            int clockwise = rotate_times > 0 ? 0 : 1;

            int cur_index = ori_index;
            for (int i = 0; i < Mathf.Abs( rotate_times ); i++)
            {
                cur_index = rotate_once[cur_index][rotate_axis][clockwise];
            }
            return cur_index;
        }

        public static int[] RotateCube(int[] ori_cube, int rotate_axis, int rotate_times)
        {
            var new_cube = new int[6];
            for (int old_index = 0; old_index < 6; old_index++)
            {
                var new_index = RotateSurface( old_index, rotate_axis, rotate_times );
                new_cube[new_index] = ori_cube[old_index];
            }
            return new_cube;
        }

        public static int UpIndex(int[] cube)
        {
            return cube.ToList().IndexOf( 2 );
        }

        public static int ForwardIndex(int[] cube)
        {
            return cube.ToList().IndexOf( 4 );
        }

        public static SurfaceNormalDirection Up(int[] cube)
        {
            var up_index = UpIndex( cube );
            return IndexToNormal( up_index );
        }
        public static SurfaceNormalDirection forward(int[] cube)
        {
            var forward_index = ForwardIndex( cube );
            return IndexToNormal( forward_index );
        }

        public static Quaternion GetCubeRotation(int[] cube)
        {
            var UpVector = index2vector3d[UpIndex( cube )];
            var ForwardVector = index2vector3d[ForwardIndex( cube )];
            return Quaternion.LookRotation( ForwardVector, UpVector );
        }

        public static int[][] Generate4Cubes(int[] ori_cube)
        {
            var cube_list = new List<int[]>();
            var up = Up( ori_cube );
            cube_list.Add( RotateCube( ori_cube, up.axis, 0 ) );
            cube_list.Add( RotateCube( ori_cube, up.axis, 1 ) );
            cube_list.Add( RotateCube( ori_cube, up.axis, 2 ) );
            cube_list.Add( RotateCube( ori_cube, up.axis, 3 ) );
            return cube_list.ToArray();
        }

        public static int[][] GenerateOriginal24CubesList()
        {
            var cube_list = new List<int[]>();
            var ori_cube = Enumerable.Range( 0, 6 ).ToArray();
            cube_list.AddRange( Generate4Cubes( ori_cube ) );
            cube_list.AddRange( Generate4Cubes( RotateCube( ori_cube, 0, 1 ) ) );
            cube_list.AddRange( Generate4Cubes( RotateCube( ori_cube, 0, -1 ) ) );
            cube_list.AddRange( Generate4Cubes( RotateCube( ori_cube, 2, 1 ) ) );
            cube_list.AddRange( Generate4Cubes( RotateCube( ori_cube, 2, -1 ) ) );
            cube_list.AddRange( Generate4Cubes( RotateCube( ori_cube, 0, 2 ) ) );
            return cube_list.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="ori_cube_list"></param>
        /// <returns>The array can be used by up and forward. [up*6][forward*4]</returns>
        public static int[][][] Sort24Cubes(int[][] ori_cube_list)
        {
            var orientation_24_cube = new int[6][][];
            for (int i = 0; i < 6; i++)
            {
                orientation_24_cube[i] = new int[6][];
            }
            foreach (int[] cube in ori_cube_list)
            {
                orientation_24_cube[UpIndex( cube )][ForwardIndex( cube )] = cube;
            }
            return orientation_24_cube;
        }


        public static Quaternion LookRotation(int up_index, int forward_index)
        {
            return Quaternion.LookRotation(
                index2vector3d[forward_index],
                index2vector3d[up_index] );
        }

        public static string Generate24CubeTableCode(int[][][] table)
        {
            var table_string = "";
            foreach (var forward_table in table)
            {
                var forward_table_string = "";
                foreach (int[] cube in forward_table)
                {
                    forward_table_string +=
                        "\t\t{" +
                        string.Join( ",", cube ?? new int[] { } ) +
                        "},\n";
                }
                table_string +=
                    "\t{\n" +
                    forward_table_string +
                    "\t},\n";
            }
            table_string =
                "{\n" +
                table_string +
                "}\n";
            return table_string;
        }


        public VoxelSurfaceUV(string source_cube_path)
        {
            source_cube = Resources.Load<Mesh>( source_cube_path );
            Generate24CubesTable();
            GenerateSurfaceTable();
        }

        /// <summary>
        ///     The cube used to generate all the surface of this generator.z
        /// </summary>
        Mesh source_cube;

        /// <summary>
        ///     Each element in each rotated new cube contains
        ///     the original surface index it holds from the source cube.
        ///     [up*6][forward*4][surface_index]
        /// </summary>
        int[][][] orientation_cube_table;

        /// <summary>
        ///     The final result of this that can be used to get any surface of a cube with any rotation.
        ///     [up][forward][surface_index]
        /// </summary>
        (Vector3[] vertices, Vector4[] uv)[][][] surface_table;


        void Generate24CubesTable()
        {
            orientation_cube_table = Sort24Cubes( GenerateOriginal24CubesList() );
        }


        void GenerateSourceSurface(
            int surface_index,
            out List<Vector3> vertices,
            out List<Vector4> uv)
        {
            var source_start_index = surface_index * 6;
            vertices = new List<Vector3>();
            uv = new List<Vector4>();
            source_cube.GetVertices( vertices );
            vertices = vertices.GetRange( source_start_index, 6 );
            source_cube.GetUVs( 0, uv );
            uv = uv.GetRange( source_start_index, 6 );
        }


        public bool GenerateSurface(
            int surface_index,
            int up_index,
            int forward_index,
            out Vector3[] vertices,
            out Vector4[] uv)
        {
            var new_cube = orientation_cube_table[up_index][forward_index];

            //The up and forward situation does not exist
            if (new_cube == null)
            {
                vertices = null;
                uv = null;
                return false;
            }

            int ori_surface_index = new_cube[surface_index];
            GenerateSourceSurface( ori_surface_index, out var ori_vertices, out var ori_uv );


            //the new vertices need to be rotated.
            vertices = new Vector3[6];
            var quaternion = LookRotation( up_index, forward_index );

            for (int i = 0; i < 6; i++)
            {
                vertices[i] = quaternion * ori_vertices[i];
            }

            //the target uv should be the uv from the position in the source cube.
            uv = ori_uv.ToArray();

            return true;
        }

        void GenerateSurfaceTable()
        {
            surface_table = new (Vector3[] vertices, Vector4[] uv)[6][][];
            for (int i = 0; i < 6; i++)
            {
                surface_table[i] = new (Vector3[] vertices, Vector4[] uv)[6][];
                for (int j = 0; j < 6; j++)
                {
                    surface_table[i][j] = new (Vector3[] vertices, Vector4[] uv)[6];
                    for (int k = 0; k < 6; k++)
                    {
                        surface_table[i][j][k] =
                            GenerateSurface(
                                k, i, j,
                                out var vertices,
                                out var uv ) ?
                                (vertices, uv) : default;
                    }
                }
            }
        }

        public void GetSurface(
            int surface_index,
            int up_index,
            int forward_index,
            out Vector3[] vertices,
            out Vector4[] uv)
        {
            (vertices, uv) = surface_table[up_index][forward_index][surface_index];
        }


    }

}
