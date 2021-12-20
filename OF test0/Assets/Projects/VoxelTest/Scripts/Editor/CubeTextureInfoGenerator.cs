using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace VoxelTest
{

    [Serializable]
    /// <summary>
    ///     Read a prototype mesh and generate the 24 rotated version uv and surface indices of it.
    /// </summary>
    public class CubeTextureInfoGenerator : EditorWindow
    {
        [MenuItem( "VoxelTest/CubeTextureInfoGenerator" )]
        static void ShowWindow()
        {
            var window = GetWindow<CubeTextureInfoGenerator>();
            window.titleContent = new GUIContent( "CubeTextureInfoGenerator" );
            window.Show();
        }

    #region Serialized

        SerializedObject this_;
        SerializedProperty m_mesh_;

    #endregion

        [SerializeField]
        Mesh m_mesh;

        void OnEnable()
        {
            m_mesh = new Mesh();
            this_ = new SerializedObject( this );
            m_mesh_ = this_.FindProperty( nameof(m_mesh) );
        }

        void OnGUI()
        {
            EditorGUILayout.PropertyField( m_mesh_ );
            this_.ApplyModifiedProperties();

            if (GUILayout.Button( "Generate" ))
            {
                Generate_indices();
                Generate_UVs();
            }
        }

        void Generate_indices()
        {
            var table = SurfaceNormalDirection.Generate24CubesTable();
            var table_string = SurfaceNormalDirection.Generate24CubeTableCode( table );

            Debug.Log( table_string );
        }

        void Generate_UVs() { }

    }


    public struct SurfaceNormalDirection
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

        /// <summary>
        ///     The normal of a cube in the index order.
        /// </summary>
        static int[][] index2normal =
        {
            new[] { 0, 0 },
            new[] { 0, 1 },
            new[] { 1, 0 },
            new[] { 1, 1 },
            new[] { 2, 0 },
            new[] { 2, 1 }
        };


        /// <summary>
        ///     The index that a normal direction maps to.
        /// </summary>
        static int[][] normal2index =
        {
            new[] { 0, 1 },
            new[] { 2, 3 },
            new[] { 4, 5 }
        };


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

        public static SurfaceNormalDirection IndexToNormal(int index)
        {
            var value = index2normal[index];
            return new SurfaceNormalDirection( value[0], value[1] );
        }

        public static int NormalToIndex(SurfaceNormalDirection direction)
        {
            return normal2index[direction.axis][direction.positive];
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

        public static int[][][] Generate24CubesTable()
        {
            return Sort24Cubes( GenerateOriginal24CubesList() );
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
                        "\t\t" +
                        string.Join( ",", cube ?? new int[] { } ) +
                        ",\n";
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
    }
}
