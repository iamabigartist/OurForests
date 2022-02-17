using Unity.Mathematics;
using UnityEngine;
namespace MUtility
{
    public static class VoxelGenerationUtility
    {

    #region Rotation

        /// <summary>
        ///     The normal of a source_cube in the index order.
        /// </summary>
        public static int[][] index2normal =
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
        public static int[][] normal2index =
        {
            new[] { 0, 1 },
            new[] { 2, 3 },
            new[] { 4, 5 }
        };


        /// <summary>
        ///     Indicate the surface order of our cube
        ///     <remarks>
        ///         <para>1. The face indices can represent the real direction of a face.</para>
        ///         <para>2. It can also represents a unique id of a face itself. </para>
        ///     </remarks>
        /// </summary>
        public static Vector3[] index2vector3d =
        {
            Vector3.right,
            Vector3.left,
            Vector3.up,
            Vector3.down,
            Vector3.forward,
            Vector3.back
        };


        public static Quaternion LookRotation(int up_index, int forward_index)
        {
            return Quaternion.LookRotation(
                index2vector3d[forward_index],
                index2vector3d[up_index] );
        }

        public static bool ValidLookRotation(int up_index, int forward_index)
        {
            return Vector3.Dot( index2vector3d[up_index], index2vector3d[forward_index] ) == 0;
        }

    #endregion

    #region MeshGeneration

    #region UV

        //The four corner uv of any quad,
        //The order is as below:
        /*
         *  1-----3
         *  |     |
         *  |     |
         *  0-----2
         */
        public static readonly float2[] uv_4p =
        {
            new int2( 0, 0 ), //00
            new int2( 0, 1 ), //01
            new int2( 1, 0 ), //10
            new int2( 1, 1 ) //11
        };

    #endregion

    #region Quad

        //The point is (x,y,z)
        //(0,0,0) is the original point of the cube
        //Every branch in a for will build a quad of the cube
        //A quad is between 2 cube corner c0 and the further side adjacent corner of c0
        //called c1 ( can be up, right, forward )
        //The first point is c0, the second point is c1
        //There are 12 quads in a cube at most.
        //This array represents the 2 cube corners' index offset of each quad.
        //
        /// <summary>
        ///     <para>The indices: [quad number][c0/c1]</para>
        ///     <para>total: [12][2]</para>
        /// </summary>
        public static readonly int3[][] quad_2_corner_in_cube =
        {

            new[] { new int3( 0, 0, 0 ), new int3( 0, 0, 1 ) },
            new[] { new int3( 0, 0, 0 ), new int3( 0, 1, 0 ) },
            new[] { new int3( 0, 0, 0 ), new int3( 1, 0, 0 ) },

            new[] { new int3( 0, 0, 1 ), new int3( 0, 1, 1 ) },
            new[] { new int3( 0, 0, 1 ), new int3( 1, 0, 1 ) },

            new[] { new int3( 0, 1, 0 ), new int3( 0, 1, 1 ) },
            new[] { new int3( 0, 1, 0 ), new int3( 1, 1, 0 ) },

            new[] { new int3( 1, 0, 0 ), new int3( 1, 0, 1 ) },
            new[] { new int3( 1, 0, 0 ), new int3( 1, 1, 0 ) },

            new[] { new int3( 0, 1, 1 ), new int3( 1, 1, 1 ) },

            new[] { new int3( 1, 0, 1 ), new int3( 1, 1, 1 ) },

            new[] { new int3( 1, 1, 0 ), new int3( 1, 1, 1 ) }
        };

        //This array represents the directions of the 12 quads. 0/1/2 -> x/y/z axis direction
        //
        /// <summary>
        ///     <para>The index: [quad number]</para>
        ///     <para>total: [12]</para>
        /// </summary>
        public static readonly int[] quad_direction_in_cube =
        {
            2, 1, 0,
            1, 0,
            2, 0,
            2, 1,
            0,
            1,
            2
        };

        //1. The point is (x,y,z),
        //A quad is between 2 cube corner c0 and the further side adjacent corner of c0\n
        //called c1 ( can be up, right, forward )
        //2. The original corner of the quad is always c1,
        //while the other 3 corner has a offset from c1
        //3. There are 6 case in total, whose quad face normal is +/- x/y/z axis direction.
        //4. When c0 is inside the mesh and c1 is outside the mesh,
        //the quad face normal should be positive axis direction
        //5. Look above the quad face, the corners should be clockwise
        //6. Note that the order of quad corner of y normal is inverse with other 2 to make it clockwise
        /// <summary>
        ///     <para>The indices: [normal direction x/y/z][normal direction +/-][which point of the quad(in order for rendering)]</para>
        ///     <para>total: [3][2][4]</para>
        /// </summary>
        public static readonly int3[][][] corner_index_offset_in_quad =
        {

            //quad of x normal
            new[]
            {
                new[] { new int3( 0, 0, 0 ), new int3( 0, 1, 0 ), new int3( 0, 1, 1 ), new int3( 0, 0, 1 ) },
                new[] { new int3( 0, 0, 0 ), new int3( 0, 0, 1 ), new int3( 0, 1, 1 ), new int3( 0, 1, 0 ) }
            },

            //quad of y normal
            new[]
            {
                new[] { new int3( 0, 0, 0 ), new int3( 0, 0, 1 ), new int3( 1, 0, 1 ), new int3( 1, 0, 0 ) },
                new[] { new int3( 0, 0, 0 ), new int3( 1, 0, 0 ), new int3( 1, 0, 1 ), new int3( 0, 0, 1 ) }
            },

            //quad of z normal
            new[]
            {
                new[] { new int3( 0, 0, 0 ), new int3( 1, 0, 0 ), new int3( 1, 1, 0 ), new int3( 0, 1, 0 ) },
                new[] { new int3( 0, 0, 0 ), new int3( 0, 1, 0 ), new int3( 1, 1, 0 ), new int3( 1, 0, 0 ) }
            }
        };


        //The uv index of every point in a quad, the order is identical to corner_index_offset_in_quad.
        /// <summary>
        ///     <para>The indices: [normal direction x/y/z][normal direction +/-][which point of the quad(in order for rendering)]</para>
        ///     <para>total: [3][2][4]</para>
        /// </summary>
        public static readonly int[][][] corner_uv_index_in_quad =
        {

            //quad of x normal
            new[]
            {
                new[] { 0, 1, 3, 2 },
                new[] { 2, 0, 1, 3 }
            },

            //quad of y normal
            new[]
            {
                new[] { 0, 1, 3, 2 },
                new[] { 1, 3, 2, 0 }
            },

            //quad of z normal
            new[]
            {
                new[] { 2, 0, 1, 3 },
                new[] { 0, 1, 3, 2 }
            }
        };


        //Clock wise
        /*
         *  A-----B
         *   \   /
         *    \ /
         *     C
         */
        public struct Triangle
        {
            public float3 A;
            public float3 B;
            public float3 C;

            public Triangle(float3 a, float3 b, float3 c)
            {
                A = a;
                B = b;
                C = c;
            }
        };


        public struct Quad
        {
            public Triangle T00;
            public Triangle T11;

            public Quad(Triangle t00, Triangle t11)
            {
                T00 = t00;
                T11 = t11;
            }


        };

        //Clock wise
        /*
         * B---C
         * |  /|
         * | / |
         * A---D
         *
         * Axis:
         *
         * ^y
         * |
         * |     x
         * O----->
         *
         * ABC Triangle00
         * CDA Triangle11
         */
        public struct QuadMaker
        {
            float3 A;
            float3 B;
            float3 C;
            float3 D;

            public QuadMaker(float3 a, float3 b, float3 c, float3 d)
            {
                A = a;
                B = b;
                C = c;
                D = d;
            }

            public QuadMaker(float3[] positions)
            {
                A = positions[0];
                B = positions[1];
                C = positions[2];
                D = positions[3];
            }

            Triangle Triangle00()
            {
                return new Triangle( A, B, C );
            }

            Triangle Triangle11()
            {
                return new Triangle( C, D, A );
            }

            public void ToTriangles(out Triangle t00, out Triangle t11)
            {
                t00 = Triangle00();
                t11 = Triangle11();
            }

            public Quad ToQuad()
            {
                return new Quad(
                    Triangle00(),
                    Triangle11() );
            }

            public float3[] ToVertices()
            {
                return new[] { A, B, C, C, D, A };
            }
        };

        //The vertex order used in quad maker to build 2 triangles.
        //This order must be used in all arrays that store info of vertices.
        public static readonly int[] triangle_order_in_quad = { 0, 1, 2, 2, 3, 0 };

    #endregion

    #endregion

    }
}
