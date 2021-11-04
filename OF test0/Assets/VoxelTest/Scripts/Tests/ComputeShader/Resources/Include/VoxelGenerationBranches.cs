using Unity.Mathematics;
namespace VoxelTest.Tests.Include
{
    static class VoxelGenerationBranches
    {
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
        static readonly int3[,] quad_2_corner_in_cube =
        {

            { new int3( 0, 0, 0 ), new int3( 0, 0, 1 ) },
            { new int3( 0, 0, 0 ), new int3( 0, 1, 0 ) },
            { new int3( 0, 0, 0 ), new int3( 1, 0, 0 ) },

            { new int3( 0, 0, 1 ), new int3( 0, 1, 1 ) },
            { new int3( 0, 0, 1 ), new int3( 1, 0, 1 ) },

            { new int3( 0, 1, 0 ), new int3( 0, 1, 1 ) },
            { new int3( 0, 1, 0 ), new int3( 1, 1, 0 ) },

            { new int3( 1, 0, 0 ), new int3( 1, 0, 1 ) },
            { new int3( 1, 0, 0 ), new int3( 1, 1, 0 ) },

            { new int3( 0, 1, 1 ), new int3( 1, 1, 1 ) },

            { new int3( 1, 0, 1 ), new int3( 1, 1, 1 ) },

            { new int3( 1, 1, 0 ), new int3( 1, 1, 1 ) }
        };

        //This array represents the directions of the 12 quads. 0/1/2 -> x/y/z axis direction
        //
        /// <summary>
        ///     <para>The index: [quad number]</para>
        ///     <para>total: [12]</para>
        /// </summary>
        static readonly int[] quad_direction_in_cube =
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
        //5. Look towards the quad face, the corners should be clockwise
        //6. Note that the order of quad corner of y normal is inverse with other 2 to make it clockwise
        /// <summary>
        ///     <para>The indices: [normal direction x/y/z][normal direction +/-][which point of the quad(in order for rendering)]</para>
        ///     <para>total: [3][2][4]</para>
        /// </summary>
        static readonly int3[,,] corner_index_offset_in_quad =
        {

            //quad of x normal
            {
                { new int3( 0, 0, 0 ), new int3( 0, 1, 0 ), new int3( 0, 1, 1 ), new int3( 0, 0, 1 ) },
                { new int3( 0, 0, 0 ), new int3( 0, 0, 1 ), new int3( 0, 1, 1 ), new int3( 0, 1, 0 ) }
            },

            //quad of y normal
            {
                { new int3( 0, 0, 0 ), new int3( 0, 0, 1 ), new int3( 1, 0, 1 ), new int3( 1, 0, 0 ) },
                { new int3( 0, 0, 0 ), new int3( 1, 0, 0 ), new int3( 1, 0, 1 ), new int3( 0, 0, 1 ) }
            },

            //quad of z normal
            {
                { new int3( 0, 0, 0 ), new int3( 1, 0, 0 ), new int3( 1, 1, 0 ), new int3( 0, 1, 0 ) },
                { new int3( 0, 0, 0 ), new int3( 0, 1, 0 ), new int3( 1, 1, 0 ), new int3( 1, 0, 0 ) }
            }
        };
    }
}
