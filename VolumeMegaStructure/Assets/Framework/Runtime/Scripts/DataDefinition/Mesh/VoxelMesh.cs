using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
using VolumeMegaStructure.Manage;
using Object = UnityEngine.Object;
namespace VolumeMegaStructure.DataDefinition.Mesh
{
	using UnityMesh = UnityEngine.Mesh;
	public class VoxelMesh : IDisposable
	{
		const MeshUpdateFlags FAST_SET_FLAG =
			MeshUpdateFlags.DontValidateIndices |
			MeshUpdateFlags.DontNotifyMeshUsers |
			MeshUpdateFlags.DontRecalculateBounds;
		const int QUAD_ARRAY_CAPACITY = 3 * 100 * 100 * 101;

		public UnityMesh unity_mesh;
		DataMatrix<VolumeUnit> volume_matrix;
		DataMatrix<bool> volume_inside_matrix;
		Dictionary<QuadMark, int> quad_index_by_quad_mark;

	#region GenIntermediate

		NativeList<QuadMark> quad_mark_list;
		ComputeBuffer quad_unit_buffer;

	#endregion

		public VoxelMesh(DataMatrix<VolumeUnit> volume_matrix, DataMatrix<bool> volume_inside_matrix)
		{
			unity_mesh = new();
			this.volume_matrix = volume_matrix;
			this.volume_inside_matrix = volume_inside_matrix;
			quad_mark_list = new(QUAD_ARRAY_CAPACITY, Allocator.Persistent);
		}

		public void InitGenerate()
		{
			int volume_count = volume_matrix.Count;

			var gen_quad_mark_list_job = new GenQuadMarkList()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix,
				quad_mark_list = quad_mark_list.AsParallelWriter()
			};
			gen_quad_mark_list_job.Schedule(volume_count, 1).Complete();
			//有可能做一个返回JobHandle的携程？

			quad_index_by_quad_mark = quad_mark_list.ToArray().
				Select((quad_mark, index) => (index, quad_mark)).
				ToDictionary(pair => pair.quad_mark, pair => pair.index);

			int quad_count = quad_mark_list.Length;

			quad_unit_buffer = new(quad_count * 7, sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.SubUpdates);
			var gen_quad_unit_array_job = new GenQuadUnitArray()
			{
				volume_matrix = volume_matrix,
				quad_mark_array = quad_mark_list.AsArray(),
				quad_unit_array = quad_unit_buffer.BeginWrite<float>(0, quad_count * 7)
			};
			gen_quad_unit_array_job.Schedule(quad_count, 1).Complete();
			quad_unit_buffer.EndWrite<float>(quad_count * 7);

			unity_mesh.SetIndexBufferParams(quad_count * 6, IndexFormat.UInt32);
			unity_mesh.SetVertexBufferParams(quad_count * 4,
				new VertexAttributeDescriptor(
					VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(
					VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));
			unity_mesh.indexBufferTarget |= GraphicsBuffer.Target.Structured;
			unity_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Structured;

			GraphicsBuffer index_buffer = unity_mesh.GetIndexBuffer();
			GraphicsBuffer vertex_buffer = unity_mesh.GetVertexBuffer(0);

			ComputeShader gen_quad6_index_buffer = Object.Instantiate(MainManager.voxel_render_manager.gen_quad6_index_buffer);
			ComputeBuffer gen_quad6_index_buffer_wait = new ComputeBuffer(1, sizeof(int));
			gen_quad6_index_buffer.SetInt("quad_count", quad_count);
			gen_quad6_index_buffer.SetBuffer(0, "render_index_buffer", index_buffer);
			gen_quad6_index_buffer.Dispatch(0,
				Mathf.CeilToInt(quad_count / 1024f), 1, 1);

			ComputeShader quad_gen_unit_to_vertex_buffer = Object.Instantiate(MainManager.voxel_render_manager.quad_gen_unit_to_vertex_buffer);
			ComputeBuffer quad_gen_unit_to_vertex_buffer_wait = new ComputeBuffer(1, sizeof(int));
			quad_gen_unit_to_vertex_buffer.SetInt("quad_count", quad_count);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "quad_unit_array", quad_unit_buffer);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "vertex_buffer", vertex_buffer);
			quad_gen_unit_to_vertex_buffer.Dispatch(0,
				Mathf.CeilToInt(quad_count / 1024f), 1, 1);

			AsyncGPUReadback.Request(quad_gen_unit_to_vertex_buffer_wait);
			AsyncGPUReadback.Request(gen_quad6_index_buffer_wait);
			AsyncGPUReadback.WaitAllRequests();

			unity_mesh.subMeshCount = 1;
			unity_mesh.SetSubMesh(0, new(0, quad_count * 6), FAST_SET_FLAG);
			unity_mesh.bounds = new(Vector3.one * 50, Vector3.one * 100 /*这个是50还是100？*/);

		}

		public void Dispose()
		{
			volume_matrix.Dispose();
			volume_inside_matrix.Dispose();
			quad_mark_list.Dispose();
			quad_unit_buffer.Release();
		}

	}
}