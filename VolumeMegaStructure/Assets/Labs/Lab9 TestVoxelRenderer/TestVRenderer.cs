using Labs.Lab3_TestGPUBuffer;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMegaStructure.Util;
using static UnityEngine.Mesh;
using static VolumeMegaStructure.Manage.MainManager;
using static VolumeMegaStructure.Util.VoxelProcessUtility;
using Random = Unity.Mathematics.Random;
namespace Labs.Lab9_TestVoxelRenderer
{
	public class TestVRenderer : MonoBehaviour
	{

	#region Config

		const MeshUpdateFlags FAST_SET_FLAG =
			MeshUpdateFlags.DontValidateIndices |
			MeshUpdateFlags.DontNotifyMeshUsers |
			MeshUpdateFlags.DontRecalculateBounds;
		public int quad_count = 100;
		public int vertex_count => quad_count * 4;
		public int render_index_count => quad_count * 6;

	#endregion

	#region Data

		Texture2DArray texture_2d_array;

	#endregion

	#region Reference

		GameObject v4_voxel_mesh_game_object;
		GameObject composed_voxel_mesh_game_object;

	#endregion

	#region Process

	#region Init

		void Init2GameObject()
		{
			v4_voxel_mesh_game_object = new(nameof(v4_voxel_mesh_game_object),
				typeof(MeshFilter), typeof(MeshRenderer));
			composed_voxel_mesh_game_object = new(nameof(composed_voxel_mesh_game_object),
				typeof(MeshFilter), typeof(MeshRenderer));
			v4_voxel_mesh_game_object.transform.parent = transform;
			composed_voxel_mesh_game_object.transform.parent = transform;
		}

		void InitTexture2DArray()
		{
			texture_2d_array = Resources.LoadAll<Texture2D>("Lab9Textures").select(t => t.CPU_DXT1ToDXT5()).GenTexture2DArray();
		}

		void Init2Material()
		{
			v4_voxel_mesh_game_object.GetComponent<MeshRenderer>().material = new(Shader.Find("Shader Graphs/VoxelMeshLit_V4"));
			v4_voxel_mesh_game_object.GetComponent<MeshRenderer>().material.SetTexture("TArray", texture_2d_array);
			composed_voxel_mesh_game_object.GetComponent<MeshRenderer>().material = new(Shader.Find("Shader Graphs/VoxelMeshLit_Decompose"));
			composed_voxel_mesh_game_object.GetComponent<MeshRenderer>().material.SetTexture("TArray", texture_2d_array);
		}

	#endregion

	#region GenMesh

		static void GenRandomQuad(
			NativeArray<float3> FixedUVVertexTable, int texture_2d_array_depth, int seed,
			out NativeArray<float3> vertices, out NativeArray<int> face_uv_texture_indices)
		{
			var random = Random.CreateFromIndex((uint)seed);
			int i_up = random.NextInt(0, 6);
			int i_forward;
			do { i_forward = random.NextInt(0, 6); } while (!ValidLookRotation(i_up, i_forward));
			int i_face = random.NextInt(0, 6);
			int i_texture = random.NextInt(0, texture_2d_array_depth);
			vertices = new(4, Allocator.Temp);
			face_uv_texture_indices = new(4, Allocator.Temp);
			i_rotation_i_face_i_vertex_Compose(i_up, i_forward, i_face, 0, out var i0);
			for (int i_vertex = 0; i_vertex < 4; i_vertex++)
			{
				vertices[i_vertex] = FixedUVVertexTable[i0 + i_vertex];
				i_texture_i_face_i_vertex_Compose(i_texture, i_face, i_vertex, out var composed_index);
				face_uv_texture_indices[i_vertex] = composed_index;
			}
		}

		[BurstCompile]
		public struct GenV4VertexBufferJob : IJobParallelFor
		{
			[ReadOnly] public NativeArray<float3> fixed_uv_vertex_table;
			[ReadOnly] public int texture_2d_array_depth;
			[NativeDisableParallelForRestriction]
			[WriteOnly] public NativeArray<float4> vertex_buffer;
			public void Execute(int i_quad)
			{
				GenRandomQuad(fixed_uv_vertex_table, texture_2d_array_depth, i_quad,
					out NativeArray<float3> vertices, out NativeArray<int> face_uv_texture_indices);
				for (int i_vertex = 0; i_vertex < 4; i_vertex++)
				{

				}
				vertices.Dispose();
				face_uv_texture_indices.Dispose();
			}
		}

		[BurstCompile]
		public struct GenComposedVertexBufferJob : IJobParallelFor
		{
			[ReadOnly] public NativeArray<float3> fixed_uv_vertex_table;
			[ReadOnly] public int texture_2d_array_depth;
			[NativeDisableParallelForRestriction]
			[WriteOnly] public NativeArray<float2> vertex_buffer;
			public void Execute(int i_quad)
			{
				GenRandomQuad(fixed_uv_vertex_table, texture_2d_array_depth, i_quad,
					out NativeArray<float3> vertices, out NativeArray<int> face_uv_texture_indices);
				for (int i_vertex = 0; i_vertex < 4; i_vertex++)
				{

				}
				vertices.Dispose();
				face_uv_texture_indices.Dispose();
			}
		}

		//1. 如果只转化buffer的话，需要一个已经提前gen 好的mesh加上vertex buffer的定义,因为set vertex buffer params 函数只在mesh里面有。
		//2. 因此还是需要两个各自生成单独的mesh，在job 里面对于GenRandomQuad 函数的结果进行处理。
		//3. 写两个job吧

		(JobGenMesh.GenQuad6IndexBufferJob index_buffer_jh, GenV4VertexBufferJob vertex_buffer_jh) GenV4MeshJob(MeshData mesh_data, JobHandle depend = default)
		{
			mesh_data.SetIndexBufferParams(render_index_count, IndexFormat.UInt32);
			mesh_data.SetVertexBufferParams(vertex_count,
				new VertexAttributeDescriptor(
					VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 4));
			var index_buffer_job = new JobGenMesh.GenQuad6IndexBufferJob
			{
				render_index_buffer = mesh_data.GetIndexData<int>()
			};
			var vertex_buffer_job = new GenV4VertexBufferJob
			{
				fixed_uv_vertex_table = voxel_gen_source_tables.FixedUVVertexTable_Native,
				texture_2d_array_depth = texture_2d_array.depth,
				vertex_buffer = mesh_data.GetVertexData<float4>()
			};
			return (index_buffer_job, vertex_buffer_job);
		}

		(JobGenMesh.GenQuad6IndexBufferJob index_buffer_jh, GenComposedVertexBufferJob vertex_buffer_jh) GenComposedMeshJob(MeshData mesh_data)
		{
			mesh_data.SetIndexBufferParams(render_index_count, IndexFormat.UInt32);
			mesh_data.SetVertexBufferParams(vertex_count,
				new VertexAttributeDescriptor(
					VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));
			var index_buffer_job = new JobGenMesh.GenQuad6IndexBufferJob
			{
				render_index_buffer = mesh_data.GetIndexData<int>()
			};
			var vertex_buffer_job = new GenComposedVertexBufferJob
			{
				fixed_uv_vertex_table = voxel_gen_source_tables.FixedUVVertexTable_Native,
				texture_2d_array_depth = texture_2d_array.depth,
				vertex_buffer = mesh_data.GetVertexData<float2>()
			};
			return (index_buffer_job, vertex_buffer_job);
		}

		void SetSubMesh(MeshData mesh_data)
		{
			mesh_data.subMeshCount = 1;
			mesh_data.SetSubMesh(0, new(0, render_index_count), FAST_SET_FLAG);
		}

		(Mesh v4_mesh, Mesh composed_mesh ) Gen2Mesh()
		{
			var mesh_data_2 = AllocateWritableMeshData(2);
			var (v4_index_buffer_job, v4_vertex_buffer_job) =
				GenV4MeshJob(mesh_data_2[0]);
			var (composed_index_buffer_job, composed_vertex_buffer_job) =
				GenComposedMeshJob(mesh_data_2[1]);

			var v4_index_buffer_jh = v4_index_buffer_job.Schedule(quad_count, 1);
			var v4_vertex_buffer_jh = v4_vertex_buffer_job.Schedule(quad_count, 1, v4_index_buffer_jh);
			var composed_index_buffer_jh = composed_index_buffer_job.Schedule(quad_count, 1, v4_vertex_buffer_jh);
			var composed_vertex_buffer_jh = composed_vertex_buffer_job.Schedule(quad_count, 1, composed_index_buffer_jh);
			var final_jh = composed_vertex_buffer_jh;
			final_jh.Complete();

			SetSubMesh(mesh_data_2[0]);
			SetSubMesh(mesh_data_2[1]);
			Mesh v4_mesh = new();
			Mesh composed_mesh = new();
			ApplyAndDisposeWritableMeshData(mesh_data_2, new[] { v4_mesh, composed_mesh }, FAST_SET_FLAG);
			return (v4_mesh, composed_mesh);
		}

	#endregion

	#endregion

	#region Interface

		[ContextMenu("Regenerate")]
		void Regenerate()
		{
			 (Mesh v4_mesh, Mesh composed_mesh) = Gen2Mesh();
			v4_voxel_mesh_game_object.GetComponent<MeshFilter>().sharedMesh = v4_mesh;
			composed_voxel_mesh_game_object.GetComponent<MeshFilter>().sharedMesh = composed_mesh;
		}

	#endregion

	#region Unity Entry

		void Start()
		{
			Init2GameObject();
			InitTexture2DArray();
			Init2Material();
		}

	#endregion
	}
}