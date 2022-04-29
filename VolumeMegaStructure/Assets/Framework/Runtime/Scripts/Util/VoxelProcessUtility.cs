using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
namespace VolumeMegaStructure.Util
{
	public static class VoxelProcessUtility
	{

	#region Index

		/// <summary>
		///     Example decompose function
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static void IndexDecompose_3_CPU(int source, int size, double inverse_size, out int result, out int remain_source)
		{
			remain_source = source % size;
			result = (int)((source - remain_source) * inverse_size);
		}
		/// <summary>
		///     Arbitrary rank decompose all, inefficient because of array.
		/// </summary>
		/// <remarks>Note that size array should be the product of size.</remarks>
		static void IndexDecompose_3_CPU_All(int source, int[] size, double[] inverse_size, out int[] result)
		{
			result = new int[size.Length];
			int cur_source = source;

			for (int i = 0; i < size.Length; i++)
			{
				IndexDecompose_3_CPU(cur_source, size[i], inverse_size[i], out result[i], out cur_source);
			}
		}

		const int FORWARD_SIZE = 6;
		const float FORWARD_SIZE_INVERSE = 6;
		const int FACE_SIZE = 6;
		const float FACE_SIZE_INVERSE = 1 / 6f;
		const int VERTEX_SIZE = 4;
		const float VERTEX_SIZE_INVERSE = 1 / 4f;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void i_rotation_i_face_i_vertex_Compose(in int i_up, in int i_forward, in int i_face, in int i_vertex, out int i)
		{
			i = ((i_up * FORWARD_SIZE + i_forward) * FACE_SIZE + i_face) * VERTEX_SIZE + i_vertex;
		} //CONSIDER haven't optimised by SIMD//

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void i_rotation_i_face_i_vertex_Decompose(out int i_up, out int i_forward, out int i_face, out int i_vertex, in int i)
		{
			int remain_i = i;

			remain_i %= FORWARD_SIZE * FACE_SIZE * VERTEX_SIZE;
			i_up = (int)((i - remain_i) * (FORWARD_SIZE_INVERSE * FACE_SIZE_INVERSE * VERTEX_SIZE_INVERSE));

			remain_i %= FACE_SIZE * VERTEX_SIZE;
			i_forward = (int)((i - remain_i) * (FACE_SIZE_INVERSE * VERTEX_SIZE_INVERSE));

			remain_i %= VERTEX_SIZE;
			i_face = (int)((i - remain_i) * VERTEX_SIZE_INVERSE);

			i_vertex = remain_i;
		} //CONSIDER haven't optimised by SIMD//

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void i_texture_i_face_i_vertex_Compose(in int i_texture, in int i_face, in int i_vertex, out int i)
		{
			i = (i_texture * FACE_SIZE + i_face) * VERTEX_SIZE + i_vertex;
		} //CONSIDER haven't optimised by SIMD//

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void position_Compose(in int3 position, in int3 chunk_size, out int composed_position)
		{
			composed_position = (position.z * chunk_size.y + position.y) * chunk_size.x + position.x;
		} //CONSIDER haven't optimised by SIMD//

	#endregion

	#region Rotation

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


		/// <summary>
		///     The normal of a source_cube in the index order.
		/// </summary>
		public static readonly int[][] index_2_normal =
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
		public static readonly int[][] normal_2_index =
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
		public static readonly float3[] index_2_normal_vector3d =
		{
			Vector3.right,
			Vector3.left,
			Vector3.up,
			Vector3.down,
			Vector3.forward,
			Vector3.back
		};

		public static quaternion IndexLookRotation(int up_index, int forward_index)
		{
			return quaternion.LookRotation(
				index_2_normal_vector3d[forward_index],
				index_2_normal_vector3d[up_index]);
		}

		public static bool ValidLookRotation(int up_index, int forward_index)
		{
			return math.dot(index_2_normal_vector3d[up_index], index_2_normal_vector3d[forward_index]) == 0;
		}

		public static float3[] rotate(this float3[] array, quaternion q)
		{
			var new_array = new float3[array.Length];

			for (int i = 0; i < array.Length; i++)
			{
				new_array[i] = math.rotate(q, array[i]);
			}

			return new_array;
		}

	#endregion

	#region MeshGeneration

	#region Index

		static readonly int[] quad_local_indices = { 0, 1, 2, 2, 3, 0 };
		public static int[] GenQuadIndices(int quad_count)
		{
			var quad_indices = new int[quad_count * 6];

			for (int quad_i = 0; quad_i < quad_count; quad_i++)
			{
				var vertex_i0 = quad_i * 6;
				var render_i0 = quad_i * 4;

				for (int local_vertex_i = 0; local_vertex_i < 6; local_vertex_i++)
				{
					var vertex_i = vertex_i0 + local_vertex_i;
					var render_i = quad_local_indices[local_vertex_i] + render_i0;
					quad_indices[vertex_i] = render_i;
				}
			}

			return quad_indices;
		}

	#endregion

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
			new int2(0, 0), //00
			new int2(0, 1), //01
			new int2(1, 0), //10
			new int2(1, 1) //11
		};

		//The four corner uv of any quad, used to gen the uv array.
		//The order is as below:
		/*
		 *  1-----2
		 *  |     |
		 *  |     |
		 *  0-----3
		 */
		public static readonly float2[] uv_4p_gen =
		{
			new int2(0, 0),
			new int2(0, 1),
			new int2(1, 1),
			new int2(1, 0)
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

			new[] { new int3(0, 0, 0), new int3(0, 0, 1) },
			new[] { new int3(0, 0, 0), new int3(0, 1, 0) },
			new[] { new int3(0, 0, 0), new int3(1, 0, 0) },

			new[] { new int3(0, 0, 1), new int3(0, 1, 1) },
			new[] { new int3(0, 0, 1), new int3(1, 0, 1) },

			new[] { new int3(0, 1, 0), new int3(0, 1, 1) },
			new[] { new int3(0, 1, 0), new int3(1, 1, 0) },

			new[] { new int3(1, 0, 0), new int3(1, 0, 1) },
			new[] { new int3(1, 0, 0), new int3(1, 1, 0) },

			new[] { new int3(0, 1, 1), new int3(1, 1, 1) },

			new[] { new int3(1, 0, 1), new int3(1, 1, 1) },

			new[] { new int3(1, 1, 0), new int3(1, 1, 1) }
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
		public static readonly int3[][][] vertex_corner_offset_in_quad =
		{

			//quad of x normal
			new[]
			{
				new[] { new int3(0, 0, 0), new int3(0, 1, 0), new int3(0, 1, 1), new int3(0, 0, 1) },
				new[] { new int3(0, 0, 0), new int3(0, 0, 1), new int3(0, 1, 1), new int3(0, 1, 0) }
			},

			//quad of y normal
			new[]
			{
				new[] { new int3(0, 0, 0), new int3(0, 0, 1), new int3(1, 0, 1), new int3(1, 0, 0) },
				new[] { new int3(0, 0, 0), new int3(1, 0, 0), new int3(1, 0, 1), new int3(0, 0, 1) }
			},

			//quad of z normal
			new[]
			{
				new[] { new int3(0, 0, 0), new int3(1, 0, 0), new int3(1, 1, 0), new int3(0, 1, 0) },
				new[] { new int3(0, 0, 0), new int3(0, 1, 0), new int3(1, 1, 0), new int3(1, 0, 0) }
			}
		};


		//The uv index of every point in a quad, the order is identical to vertex_corner_offset_in_quad.
		/// <summary>
		///     <para>The indices: [normal direction x/y/z][normal direction +/-][which point of the quad(in order for rendering)]</para>
		///     <para>total: [3][2][4]</para>
		/// </summary>
		public static readonly int[][][] vertex_uv_index_in_quad =
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
				return new(A, B, C);
			}

			Triangle Triangle11()
			{
				return new(C, D, A);
			}

			public void ToTriangles(out Triangle t00, out Triangle t11)
			{
				t00 = Triangle00();
				t11 = Triangle11();
			}

			public Quad ToQuad()
			{
				return new(
					Triangle00(),
					Triangle11());
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