using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VolumeMegaStructure.Util.VoxelProcessUtility;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{

    public class VoxelRotationFaceTable
    {

    #region Util

    #region Table

        /// <summary>
        ///     <para>
        ///         1. We can use the face index to represent the direction of a cube face. See
        ///         <see cref="VoxelGenerationUtility.index2normal_vector3d" />.
        ///     </para>
        ///     <para>
        ///         2. If we rotate the original direction face once, the face will turn to a new direction, the new direction can
        ///         also be represented by a face index.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     rotate_once [original_face_index][rotate_axis][clockwise/counterclockwise] = new_face_index
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

    #endregion

    #region Process

        public static SurfaceNormalDirection IndexToNormal(int index)
        {
            var value = index_2_normal[index];

            return new(value[0], value[1]);
        }

        public static int NormalToIndex(SurfaceNormalDirection direction)
        {
            return normal_2_index[direction.axis][direction.positive];
        }

        public static Vector3 NormalDirectionToVector3(SurfaceNormalDirection direction)
        {
            return index_2_normal_vector3d[NormalToIndex( direction )];
        }

        /// <summary>
        ///     Get the index of the face with id = 2, represents the original up face.
        /// </summary>
        public static int UpIndex(int[] cube)
        {
            return cube.ToList().IndexOf( 2 );
        }

        /// <summary>
        ///     Get the index of the face with id = 4, represents the original forward face.
        /// </summary>
        public static int ForwardIndex(int[] cube)
        {
            return cube.ToList().IndexOf( 4 );
        }

        public static SurfaceNormalDirection Up(int[] cube)
        {
            var up_index = UpIndex( cube );

            return IndexToNormal( up_index );
        }

        public static SurfaceNormalDirection Forward(int[] cube)
        {
            var forward_index = ForwardIndex( cube );

            return IndexToNormal( forward_index );
        }

        public static Quaternion GetCubeRotation(int[] cube)
        {
            var up_vector = index_2_normal_vector3d[UpIndex( cube )];
            var forward_vector = index_2_normal_vector3d[ForwardIndex( cube )];

            return Quaternion.LookRotation( forward_vector, up_vector );
        }

    #endregion

    #endregion

    #region Data

        /// <summary>
        ///     Each element in each rotated new cube contains
        ///     the original face index it holds from the original cube.
        ///     [up*6][forward*6][surface_index]
        /// </summary>
        int[][][] orientation_cube_table;

    #endregion

    #region Process

    #region Rotation

        /// <summary>
        ///     This function uses <see cref="rotate_once" />.
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

        /// <summary>
        ///     Rotate the totally 6 indices of the whole cube.
        ///     The direction in a face of the old index will go to the face of the new index.
        /// </summary>
        /// <returns>the new face indices of the cube.</returns>
        public static int[] RotateCube(int[] ori_cube, int rotate_axis, int rotate_times)
        {
            var new_cube = new int[6];

            for (int old_index = 0; old_index < 6; old_index++)
            {
                var new_index = RotateSurface( old_index, rotate_axis, rotate_times );
                //Array transform
                new_cube[new_index] = ori_cube[old_index];
            }

            return new_cube;
        }

    #endregion

    #region Generate

        /// <summary>
        ///     Generate the 4 cube rotated 0~3 times around its up axis.
        /// </summary>
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

        /// <summary>
        ///     Generate the 24 cube with different up indices in group of 4 by <see cref="Generate4Cubes" />
        /// </summary>
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

        /// <returns>The array can be used by up and forward. </returns>
        /// <remarks>
        ///     [up_dir*6][forward_dir*6][face_dir]
        /// </remarks>
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

        void Generate24CubesTable()
        {
            orientation_cube_table = Sort24Cubes( GenerateOriginal24CubesList() );
        }

    #endregion

    #region Serialise

        [Obsolete( "Manually Serialised, Hard to deserialize, should use json later" )]
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

        public static string Serialise(int[][][] cube_table)
        {
            throw new NotImplementedException();
        }

    #endregion

    #endregion

    #region Interface

        public VoxelRotationFaceTable()
        {
            Generate24CubesTable();
        }

        public int GetOriginalFaceDirection(int cur_face_index, int up_index, int forward_index)
        {
            return orientation_cube_table[up_index][forward_index][cur_face_index];
        }

    #endregion
    }

}
