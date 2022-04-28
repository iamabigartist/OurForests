using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;
namespace Labs.Lab3_TestGPUBuffer
{
	public class JobGenMesh : MonoBehaviour
	{
	#region Job

		[BurstCompile]
		public struct GenQuad6IndexBufferJob : IJobParallelFor
		{
			[ReadOnly] static readonly int[] LOCAL_INDEX_BUFFER = { 0, 1, 2, 2, 3, 0 };
			[NativeDisableParallelForRestriction]
			 public NativeArray<int> render_index_buffer;
			public void Execute(int quad_i)
			{
				var vertex_i0 = quad_i * 6;
				var render_i0 = quad_i * 4;

				for (int local_vertex_i = 0; local_vertex_i < 6; local_vertex_i++)
				{
					var vertex_i = vertex_i0 + local_vertex_i;
					var render_i = LOCAL_INDEX_BUFFER[local_vertex_i] + render_i0;
					render_index_buffer[vertex_i] = render_i;
				}
			}
		}

		[BurstCompile]
		public struct RandomSphereVertexBufferJob : IJobParallelFor
		{
			[ReadOnly] static readonly float3 forward = new(0, 0, 1);
			[WriteOnly] public NativeArray<float3> vertex_buffer;
			public void Execute(int vertex_i)
			{
				var random = Random.CreateFromIndex((uint)vertex_i);
				var random_normalized_vector = rotate(random.NextQuaternionRotation(), forward);
				vertex_buffer[vertex_i] = random_normalized_vector;
			}
		}

	#endregion

	#region Config

		const MeshUpdateFlags FAST_SET_FLAG =
			MeshUpdateFlags.DontValidateIndices |
			MeshUpdateFlags.DontNotifyMeshUsers |
			MeshUpdateFlags.DontRecalculateBounds;

		public int quad_count = 1000;
		public int vertex_count => quad_count * 4;
		public int flatten_index_count => vertex_count;
		public int index_count => quad_count * 6;

	#endregion

	#region Data

		Mesh m_mesh;

	#endregion

	#region Process

		[ContextMenu("GenMesh")]
		void GenMesh()
		{
			var mesh_data = Mesh.AllocateWritableMeshData(1);
			var data0 = mesh_data[0];

			data0.SetIndexBufferParams(index_count, IndexFormat.UInt32);
			var index_job = new GenQuad6IndexBufferJob
			{
				render_index_buffer = data0.GetIndexData<int>()
			};
			var index_job_handle = index_job.Schedule(quad_count, 1);

			index_job_handle.Complete();

			data0.SetVertexBufferParams(vertex_count,
				new VertexAttributeDescriptor(VertexAttribute.Position));
			var vertex_job = new RandomSphereVertexBufferJob
			{
				vertex_buffer = data0.GetVertexData<float3>()
			};
			var vertex_job_handle = vertex_job.Schedule(vertex_count, 1);

			vertex_job_handle.Complete();

			data0.subMeshCount = 1;
			data0.SetSubMesh(0, new(0, index_count), FAST_SET_FLAG);

			m_mesh = new();
			Mesh.ApplyAndDisposeWritableMeshData(mesh_data, m_mesh, FAST_SET_FLAG);
			m_mesh.RecalculateNormals();
			m_mesh.RecalculateBounds();

			m_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Structured;
			m_mesh.MarkDynamic();
			Debug.Log(m_mesh.GetVertexBuffer(0).usageFlags);
			GetComponent<MeshFilter>().mesh = m_mesh;
		}

	#endregion

	#region EntryPoint

		void Start() {}

	#endregion

	}
}