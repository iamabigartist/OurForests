using System;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static Unity.Mathematics.math;
using static VolumeMegaStructure.Util.VoxelProcessUtility;
using quaternion = Unity.Mathematics.quaternion;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     <para>The vertices used to gen quads in compute shader.</para>
	///     <para>The normals, tangents and uvs used to render</para>
	/// </summary>
	public class VoxelSourceTables : IDisposable
	{

	#region Data&Process

	#region Static

		public static readonly float3[] voxel_right_quad =
		{
			(1, -1, -1).f3(),
			(1, 1, -1).f3(),
			(1, 1, 1).f3(),
			(1, -1, 1).f3()
		};

	#endregion

	#region Intermediate

		float3[][] voxel_6_quad;

		void voxel_right_quad_2_6_quad()
		{
			voxel_6_quad = new float3[6][];
			voxel_6_quad[0] = voxel_right_quad;
			voxel_6_quad[1] = voxel_right_quad.rotate(quaternion.RotateY(radians(180)));
			voxel_6_quad[4] = voxel_right_quad.rotate(quaternion.RotateY(radians(-90)));
			voxel_6_quad[5] = voxel_right_quad.rotate(quaternion.RotateY(radians(90)));
			voxel_6_quad[2] = voxel_6_quad[4].rotate(quaternion.RotateX(radians(-90)));
			voxel_6_quad[3] = voxel_6_quad[4].rotate(quaternion.RotateX(radians(90)));
		}

		Mesh source_voxel;

		void init_source_voxel()
		{
			source_voxel = new();
			source_voxel.SetVertices(voxel_6_quad.SelectMany(v => v).ToArray().f3_2_v3());
			source_voxel.SetIndices(GenQuadIndices(6), MeshTopology.Triangles, 0);
			source_voxel.RecalculateNormals();
			source_voxel.RecalculateTangents();
		}

	#endregion

	#region Results

		float3[] face_normals;
		float4[] face_tangents;

		void source_voxel_2_normal_tangent()
		{
			face_normals = new float3[6];
			face_tangents = new float4[6];
			for (int i_face = 0; i_face < 6; i_face++)
			{
				int i_vertex0 = i_face * 4;
				face_normals[i_face] = source_voxel.normals[i_vertex0];
				face_tangents[i_face] = source_voxel.tangents[i_vertex0];
			}
		}

		float3[] fixed_uv_vertex_table;
		void voxel_6_quad_2_i_rotation_i_face_i_vertex_quads()
		{
			fixed_uv_vertex_table = new float3[6 * 6 * 6 * 4];

			for (int i_up = 0; i_up < 6; i_up++)
			{
				for (int i_forward = 0; i_forward < 6; i_forward++)
				{
					int i_rotation = i_up * 6 + i_forward;

					for (int i_face = 0; i_face < 6; i_face++)
					{
						var source_quad = voxel_6_quad[i_face];
						var rotated_quad = source_quad.@select(v => rotate(IndexLookRotation(i_up, i_forward), v));
						// Debug.Log(rotated_quad.ToMString(","));
						int i_rotation_i_face = i_rotation * 6 + i_face;

						for (int i_vertex = 0; i_vertex < 4; i_vertex++)
						{
							int i_rotation_i_face_i_vertex = i_rotation_i_face * 4 + i_vertex;
							fixed_uv_vertex_table[i_rotation_i_face_i_vertex] = rotated_quad[i_vertex];
						}
					}
				}
			}
		}

		void transform_all_quads()
		{
			for (int i = 0; i < fixed_uv_vertex_table.Length; i++)
			{
				var quad = fixed_uv_vertex_table[i];
				fixed_uv_vertex_table[i] = (quad + (1, 1, 1).f3()) / 2f;
			}

			fixed_uv_vertex_table = fixed_uv_vertex_table.round();
			// Debug.Log(fixed_uv_vertex_table.ToMString(","));
		}

		NativeArray<float3> fixed_uv_vertex_table_na;

		void GenNativeVertexTable()
		{
			fixed_uv_vertex_table_na = new(fixed_uv_vertex_table, Allocator.Persistent);
		}

	#endregion

	#endregion

	#region Entry

		public VoxelSourceTables()
		{
			voxel_right_quad_2_6_quad();
			init_source_voxel();
			source_voxel_2_normal_tangent();
			voxel_6_quad_2_i_rotation_i_face_i_vertex_quads();
			transform_all_quads();
			GenNativeVertexTable();
		}

		public float3[] FaceNormals => face_normals;
		public float4[] FaceTangents => face_tangents;
		public float3[] FixedUVVertexTable => fixed_uv_vertex_table;
		public NativeArray<float3> FixedUVVertexTable_Native => fixed_uv_vertex_table_na;

		public void GetVoxelSourceRenderBuffers(
			out ComputeBuffer source_vertex_buffer,
			out ComputeBuffer uv_buffer,
			out ComputeBuffer normal_buffer,
			out ComputeBuffer tangent_buffer)
		{
			source_vertex_buffer = new(fixed_uv_vertex_table.Length, sizeof(float) * 3, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
			source_vertex_buffer.SetData(fixed_uv_vertex_table);

			uv_buffer = new(4, sizeof(float) * 2, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
			uv_buffer.SetData(uv_4p_gen);

			normal_buffer = new(6, sizeof(float) * 3, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
			normal_buffer.SetData(face_normals);

			tangent_buffer = new(6, sizeof(float) * 4, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
			tangent_buffer.SetData(face_tangents);

		}

		public void Dispose()
		{
			fixed_uv_vertex_table_na.Dispose();
		}

	#endregion

	}
}