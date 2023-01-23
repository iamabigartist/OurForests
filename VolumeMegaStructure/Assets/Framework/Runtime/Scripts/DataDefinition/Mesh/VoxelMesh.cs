using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense.Greedy;
using VolumeMegaStructure.Manage;
using VolumeMegaStructure.Util;
using VolumeMegaStructure.Util.JobSystem.Jobs;
using static VolumeMegaStructure.Util.DisposeUtil;
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

		public UnityMesh unity_mesh;
		DataMatrix<ushort> volume_matrix;
		DataMatrix<bool> volume_inside_matrix;
		Dictionary<QuadMark, int> quad_index_by_quad_mark;

	#region GenIntermediate

	#endregion

	#region Debug

		float[] quad_unit_array;
		float[] vertex_buffer_array;
		int[] index_buffer_array;

	#endregion

		public VoxelMesh(DataMatrix<ushort> volume_matrix, DataMatrix<bool> volume_inside_matrix)
		{
			unity_mesh = new();
			this.volume_matrix = volume_matrix;
			this.volume_inside_matrix = volume_inside_matrix;
		}

		public void InitGenerate()
		{
			var stop_watch = new ProfileStopWatch();
			int volume_count = volume_matrix.Count;
			int3 size = volume_matrix.size;

		#region OriginalMarkScan

			// stop_watch.StartRecord("GenQuadMarkList");
			//
			// var gen_quad_mark_list_jh = GenQuadMarkList.ScheduleParallel(volume_matrix, volume_inside_matrix, max_quad_count, out quad_mark_list);
			// gen_quad_mark_list_jh.Complete();
			// //有可能做一个返回JobHandle的携程？
			//
			// stop_watch.StopRecord();

		#endregion

			stop_watch.Start("GenQuadMarkQueue");
			var gen_quad_mark_jh = GenQuadMarkQueue.ScheduleParallel(volume_inside_matrix, out var quad_mark_queue);
			gen_quad_mark_jh.Complete();
			//有可能做一个返回JobHandle的携程？
			stop_watch.Stop();

			int quad_count = quad_mark_queue.Count;

			stop_watch.Start("QuadMarkQueueToArray");
			var quad_mark_array = quad_mark_queue.ToArray(Allocator.TempJob);
			quad_mark_queue.Dispose();
			stop_watch.Stop();

			stop_watch.Start("ToMarkDict");
			quad_index_by_quad_mark = quad_mark_array.
				Select((quad_mark, index) => (index, quad_mark)).
				ToDictionary(pair => pair.quad_mark, pair => pair.index);
			stop_watch.Stop();

			stop_watch.Start("GenQuadUnitBuffer");
			var quad_unit_buffer = new ComputeBuffer(quad_count * 7, sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.SubUpdates);
			var gen_quad_unit_array_job = new GenQuadUnitArray()
			{
				volume_matrix = volume_matrix,
				quad_mark_array = quad_mark_array,
				quad_unit_array = quad_unit_buffer.BeginWrite<float>(0, quad_count * 7)
			};
			gen_quad_unit_array_job.Schedule(quad_count, 1).Complete();
			quad_unit_buffer.EndWrite<float>(quad_count * 7);
			quad_mark_array.Dispose();
			stop_watch.Stop();

			stop_watch.Start("SetMeshParam");
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
			stop_watch.Stop();

			stop_watch.Start("GenQuad6IndexBuffer");
			ComputeShader gen_quad6_index_buffer = Object.Instantiate(MainManager.voxel_render_manager.gen_quad6_index_buffer);
			ComputeBuffer gen_quad6_index_buffer_wait = new ComputeBuffer(1, sizeof(int));
			gen_quad6_index_buffer.SetInt("quad_count", quad_count);
			gen_quad6_index_buffer.SetBuffer(0, "render_index_buffer", index_buffer);
			gen_quad6_index_buffer.Dispatch(0,
				Mathf.CeilToInt(quad_count / 512f), 1, 1);
			stop_watch.Stop();

			stop_watch.Start("QuadGenUnitToVertexBuffer");
			ComputeShader quad_gen_unit_to_vertex_buffer = Object.Instantiate(MainManager.voxel_render_manager.quad_gen_unit_to_vertex_buffer);
			ComputeBuffer quad_gen_unit_to_vertex_buffer_wait = new ComputeBuffer(1, sizeof(int));
			quad_gen_unit_to_vertex_buffer.SetInt("quad_count", quad_count);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "quad_unit_array", quad_unit_buffer);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "vertex_buffer", vertex_buffer);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "wait", quad_gen_unit_to_vertex_buffer_wait);
			quad_gen_unit_to_vertex_buffer.Dispatch(0,
				Mathf.CeilToInt(quad_count / 512f), 1, 1);
			stop_watch.Stop();

			// stop_watch.Start("WaitBuffers");
			//
			// AsyncGPUReadback.Request(index_buffer);
			// AsyncGPUReadback.Request(vertex_buffer);
			// AsyncGPUReadback.WaitAllRequests();
			// stop_watch.Stop();

			stop_watch.Start("SetSubMeshAndBound");
			unity_mesh.subMeshCount = 1;
			unity_mesh.SetSubMesh(0, new(0, quad_count * 6), FAST_SET_FLAG);
			unity_mesh.bounds = new(volume_matrix.CenterPoint.v(), volume_matrix.size.v());
			stop_watch.Stop();

			stop_watch.Start("AllocateVertexArray");
			vertex_buffer_array = new float[quad_count * 4 * 5];
			stop_watch.Stop();

			stop_watch.Start("GetTransData");
			vertex_buffer.GetData(vertex_buffer_array);
			stop_watch.Stop();

			stop_watch.Start("SetTransData");
			vertex_buffer.SetData(vertex_buffer_array);
			stop_watch.Stop();

			stop_watch.Start("CopyArraysAndClean");
			// vertex_buffer_array = new float[rect_count * 4 * 5];
			// index_buffer_array = new int[quad_count * 6];
			// quad_unit_array = new float[quad_count * 7];
			// vertex_buffer.GetData(vertex_buffer_array);
			// index_buffer.GetData(index_buffer_array);
			// quad_unit_buffer.GetData(quad_unit_array);
			quad_unit_buffer.Release();
			index_buffer.Release();
			vertex_buffer.Release();
			quad_gen_unit_to_vertex_buffer_wait.Release();
			gen_quad6_index_buffer_wait.Release();
			stop_watch.Stop();

			Debug.Log(stop_watch.PrintAllRecords());

		}

		public async Task InitGenerate_Greedy()
		{
			var w = new ProfileStopWatch();
			int volume_count = volume_matrix.Count;
			int3 matrix_size = volume_matrix.size;

			///思路
			/// 数组虽然不能在job内部使用，但是可以在外部使用，在job自身的构造函数中再将数组和job中字段对应。
			/// 关于scan line，使用array和set，选取一个quad，先看前面是否断裂，再往后递归直到断裂，来选取一个line，即（原点id，纹理id，长度）。
			/// 关于不同方向quad访问方向不同，使用TWalker，每个方向一个Walker，有 walk_in_line 函数，也有 walk_among_line 函数。
			/// 一个greedy surface，即（原点id，纹理id，主方向长度，副方向长度）

			w.Start("GenDirectionQuadQueue");
			GenDirectionQuadQueue.ScheduleParallel(matrix_size, volume_count, volume_inside_matrix, out var quad_queues).Complete();
			w.Stop();

			w.Start("QuadPosQueueToArray");
			var quad_pos_arrays = quad_queues.Select(stream => stream.ToArray(Allocator.TempJob)).ToArray();
			w.Stop();

			w.Start("GenQuadUnitArray_New");
			GenQuadUnitArray_New.Plan6Dir(matrix_size, volume_matrix, quad_pos_arrays, out var quad_arrays).Complete();
			w.Stop();

			w.Start("QuadArrayToSet");
			NativeArrayToParallelHashSetForJob<int2>.ScheduleParallel(quad_arrays, out var quad_sets).Complete();
			w.Stop();

			w.Start("GreedyLine");
			ScanQuadToLine.Plan6Dir(matrix_size, quad_arrays, quad_sets, out var line_queues).Complete();
			w.Stop();

			w.Start("LineQueueToArray");
			var line_arrays = line_queues.Select(queue => queue.ToArray(Allocator.TempJob)).ToArray();
			w.Stop();

			w.Start("LineArrayToSet");
			NativeArrayToParallelHashSetForJob<int3>.ScheduleParallel(line_arrays, out var line_sets).Complete();
			w.Stop();

			w.Start("GreedyRect");
			ScanLineToRect.Plan6Dir(matrix_size, line_arrays, line_sets, out var rect_queues).Complete();
			w.Stop();

			w.Start("RectQueueToArray");
			var rect_arrays = rect_queues.Select(queue => queue.ToArray(Allocator.TempJob)).ToArray();
			w.Stop();

			w.Start("PrefixSumCollect");
			var rect_array_start_lens = new int2[6];
			int rect_count = 0;
			for (int i = 0; i < 6; i++)
			{
				var cur_len = rect_arrays[i].Length;
				rect_array_start_lens[i] = new(rect_count, cur_len);
				rect_count += cur_len;
			}
			w.Stop();

			w.Start("InitRectBuffers");
			var rect_buffers = rect_arrays.Select(array =>
			{
				int count = array.Length;
				ComputeBuffer buffer;
				if (count == 0) { buffer = null; }
				else
				{
					buffer = new(count, 3 * sizeof(int), ComputeBufferType.Structured);
					buffer.SetData(array);
				}
				return buffer;
			}).ToArray();
			w.Stop();

			w.Start("SetMeshParam");
			unity_mesh.SetIndexBufferParams(rect_count * 6, IndexFormat.UInt32);
			unity_mesh.SetVertexBufferParams(rect_count * 4,
				new VertexAttributeDescriptor(
					VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(
					VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));
			unity_mesh.indexBufferTarget |= GraphicsBuffer.Target.Structured;
			unity_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Structured;
			GraphicsBuffer index_buffer = unity_mesh.GetIndexBuffer();
			GraphicsBuffer vertex_buffer = unity_mesh.GetVertexBuffer(0);
			w.Stop();

			w.Start("GenQuad6IndexBuffer");
			ComputeShader gen_quad6_index_buffer = Object.Instantiate(MainManager.voxel_render_manager.gen_quad6_index_buffer);
			gen_quad6_index_buffer.SetInt("quad_count", rect_count);
			gen_quad6_index_buffer.SetBuffer(0, "render_index_buffer", index_buffer);
			gen_quad6_index_buffer.Dispatch(0,
				Mathf.CeilToInt(rect_count / 512f), 1, 1);
			w.Stop();

			await UniTask.Create(() =>
			{
				w.Start("SetSubMeshCount");
				unity_mesh.subMeshCount = 1;
				w.Stop();

				w.Start("SetSubMesh0Info");
				unity_mesh.SetSubMesh(0, new(0, rect_count * 6), FAST_SET_FLAG);
				w.Stop();
				return UniTask.CompletedTask;
			});

			w.Start("SetBound");
			unity_mesh.bounds = new(volume_matrix.CenterPoint.v(), volume_matrix.size.v());
			w.Stop();

			w.Start("GenRectVertexBuffer");
			GenRectVB.Plan6Dir(matrix_size, vertex_buffer, rect_array_start_lens, rect_buffers);
			w.Stop();

			w.Start("Clean");
			// index_buffer_array = new int[rect_count * 6];
			// vertex_buffer_array = new float[rect_count * 4 * 5];
			// index_buffer.GetData(index_buffer_array);
			// vertex_buffer.GetData(vertex_buffer_array);
			index_buffer.Release();
			vertex_buffer.Release();
			DisposeArray(quad_queues);
			DisposeArray(quad_pos_arrays);
			DisposeArray(quad_arrays);
			DisposeArray(quad_sets);
			DisposeArray(line_queues);
			DisposeArray(line_arrays);
			DisposeArray(line_sets);
			DisposeArray(rect_queues);
			DisposeArray(rect_arrays);
			DisposeArray(rect_buffers);
			w.Stop();

			Debug.Log(w.PrintAllRecords());

		}

		public void Dispose() {}

	}
}