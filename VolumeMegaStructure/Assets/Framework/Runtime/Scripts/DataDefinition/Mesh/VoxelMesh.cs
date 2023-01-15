using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Greedy;
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

		ComputeBuffer quad_unit_buffer;

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

		public int MatrixSectionQuadCount(int count)
		{
			return (int)math.pow(count, 2 / 3f);
		}

		public int MaxQuadCount(int3 size)
		{
			return
				(size.x + 1) * size.y * size.z +
				size.x * (size.y + 1) * size.z +
				size.x * size.y * (size.z + 1);

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

		#region QueueAndSet

			///思路
			/// 数组虽然不能在job内部使用，但是可以在外部使用，在job自身的构造函数中再将数组和job中字段对应。
			/// 关于scan line，使用array和set，选取一个quad，先看前面是否断裂，再往后递归直到断裂，来选取一个line，即（原点id，纹理id，长度）。
			/// 关于不同方向quad访问方向不同，使用TWalker，每个方向一个Walker，有 walk_in_line 函数，也有 walk_among_line 函数。
			/// 一个greedy surface，即（原点id，纹理id，主方向长度，副方向长度）

			stop_watch.StartRecord("GenDirectionQuadQueue");
			GenDirectionQuadQueue.ScheduleParallel(size, volume_count, volume_inside_matrix, out var quad_queues).Complete();
			stop_watch.StopRecord();

			stop_watch.StartRecord("QuadPosQueueToArray");
			var quad_pos_arrays = quad_queues.Select(stream => stream.ToArray(Allocator.TempJob)).ToArray();
			stop_watch.StopRecord();

			stop_watch.StartRecord("GenQuadUnitArray_New");
			GenQuadUnitArray_New.Plan6Dir(size, volume_matrix, quad_pos_arrays, out var quad_arrays).Complete();
			stop_watch.StopRecord();

			stop_watch.StartRecord("QuadArrayToSet");
			NativeArrayToHashSetForJob<int2>.ScheduleParallel(quad_arrays, out var quad_sets).Complete();
			stop_watch.StopRecord();

			stop_watch.StartRecord("GreedyLine");
			ScanQuadToLine.Plan6Dir(size, quad_arrays, quad_sets, out var line_queues).Complete();
			stop_watch.StopRecord();

			stop_watch.StartRecord("LineQueueToArray");
			var line_arrays = line_queues.Select(queue => queue.ToArray(Allocator.TempJob)).ToArray();
			stop_watch.StopRecord();

			stop_watch.StartRecord("LineArrayToSet");
			NativeArrayToHashSetForJob<int3>.ScheduleParallel(line_arrays, out var line_sets).Complete();
			stop_watch.StopRecord();

			stop_watch.StartRecord("GreedyRect");
			ScanLineToRect.Plan6Dir(size, line_arrays, line_sets, out var rect_queues).Complete();
			stop_watch.StopRecord();

			stop_watch.StartRecord("RectQueueToArray");
			var rect_arrays = rect_queues.Select(queue => queue.ToArray(Allocator.TempJob)).ToArray();
			stop_watch.StopRecord();

			var rect_buffers = rect_arrays.Select(array =>
			{
				int count = array.Length;
				var buffer = new ComputeBuffer(count, 3 * sizeof(int), ComputeBufferType.Structured, ComputeBufferMode.SubUpdates);
				buffer.SetData(array);
				return buffer;
			});

			ComputeShader rect_uni_to_vb = Resources.Load<ComputeShader>("RectUnitToVB");

			DisposeAll(quad_queues);
			DisposeAll(quad_pos_arrays);
			DisposeAll(quad_arrays);
			DisposeAll(quad_sets);
			DisposeAll(line_queues);
			DisposeAll(line_arrays);
			DisposeAll(line_sets);
			DisposeAll(rect_queues);

		#endregion

			stop_watch.StartRecord("GenQuadMarkQueue");
			var gen_quad_mark_jh = GenQuadMarkQueue.ScheduleParallel(volume_inside_matrix, out var quad_mark_queue);
			gen_quad_mark_jh.Complete();
			//有可能做一个返回JobHandle的携程？
			stop_watch.StopRecord();

			int quad_count = quad_mark_queue.Count;

			stop_watch.StartRecord("QuadMarkQueueToArray");
			var quad_mark_array = quad_mark_queue.ToArray(Allocator.TempJob);
			stop_watch.StopRecord();

			quad_mark_queue.Dispose();

			quad_index_by_quad_mark = quad_mark_array.
				Select((quad_mark, index) => (index, quad_mark)).
				ToDictionary(pair => pair.quad_mark, pair => pair.index);

			stop_watch.StartRecord("GenQuadUnitArray");

			quad_unit_buffer = new(quad_count * 7, sizeof(float), ComputeBufferType.Structured, ComputeBufferMode.SubUpdates);
			var gen_quad_unit_array_job = new GenQuadUnitArray()
			{
				volume_matrix = volume_matrix,
				quad_mark_array = quad_mark_array,
				quad_unit_array = quad_unit_buffer.BeginWrite<float>(0, quad_count * 7)
			};
			gen_quad_unit_array_job.Schedule(quad_count, 1).Complete();
			quad_unit_buffer.EndWrite<float>(quad_count * 7);

			stop_watch.StopRecord();

			quad_mark_array.Dispose();

			stop_watch.StartRecord("SetMeshParam");

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

			stop_watch.StopRecord();

			stop_watch.StartRecord("GenQuad6IndexBuffer");

			ComputeShader gen_quad6_index_buffer = Object.Instantiate(MainManager.voxel_render_manager.gen_quad6_index_buffer);
			ComputeBuffer gen_quad6_index_buffer_wait = new ComputeBuffer(1, sizeof(int));
			gen_quad6_index_buffer.SetInt("quad_count", quad_count);
			gen_quad6_index_buffer.SetBuffer(0, "render_index_buffer", index_buffer);
			gen_quad6_index_buffer.Dispatch(0,
				Mathf.CeilToInt(quad_count / 1024f), 1, 1);

			stop_watch.StopRecord();

			stop_watch.StartRecord("QuadGenUnitToVertexBuffer");

			ComputeShader quad_gen_unit_to_vertex_buffer = Object.Instantiate(MainManager.voxel_render_manager.quad_gen_unit_to_vertex_buffer);
			ComputeBuffer quad_gen_unit_to_vertex_buffer_wait = new ComputeBuffer(1, sizeof(int));
			quad_gen_unit_to_vertex_buffer.SetInt("quad_count", quad_count);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "quad_unit_array", quad_unit_buffer);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "vertex_buffer", vertex_buffer);
			quad_gen_unit_to_vertex_buffer.SetBuffer(0, "wait", quad_gen_unit_to_vertex_buffer_wait);
			quad_gen_unit_to_vertex_buffer.Dispatch(0,
				Mathf.CeilToInt(quad_count / 512f), 1, 1);

			stop_watch.StopRecord();

			stop_watch.StartRecord("WaitBuffers");

			AsyncGPUReadback.Request(index_buffer);
			AsyncGPUReadback.Request(vertex_buffer);
			AsyncGPUReadback.WaitAllRequests();
			stop_watch.StopRecord();

			stop_watch.StartRecord("CopyArraysAndClean");

			unity_mesh.subMeshCount = 1;
			unity_mesh.SetSubMesh(0, new(0, quad_count * 6), FAST_SET_FLAG);
			unity_mesh.bounds = new(volume_matrix.CenterPoint.V(), volume_matrix.size.V());

			index_buffer_array = new int[quad_count * 6];
			vertex_buffer_array = new float[quad_count * 4 * 5];
			quad_unit_array = new float[quad_count * 7];
			index_buffer.GetData(index_buffer_array);
			vertex_buffer.GetData(vertex_buffer_array);
			quad_unit_buffer.GetData(quad_unit_array);

			index_buffer.Release();
			vertex_buffer.Release();
			quad_gen_unit_to_vertex_buffer_wait.Release();
			gen_quad6_index_buffer_wait.Release();

			stop_watch.StopRecord();

			Debug.Log(stop_watch.PrintAllRecords());

		}

		public void Dispose()
		{
			quad_unit_buffer.Release();
		}

	}
}