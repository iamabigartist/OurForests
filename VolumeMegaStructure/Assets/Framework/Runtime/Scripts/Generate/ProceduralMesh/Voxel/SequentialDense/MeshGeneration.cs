using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense;
using VolumeMegaStructure.Util;
using static Cysharp.Threading.Tasks.UniTask;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	public static class MeshGeneration
	{

		const MeshUpdateFlags FAST_SET_FLAG =
			MeshUpdateFlags.DontValidateIndices |
			MeshUpdateFlags.DontNotifyMeshUsers |
			MeshUpdateFlags.DontRecalculateBounds;

		public static async UniTask<Mesh> GenMeshAsync(
			NativeArray<int> index_array,
			int3 chunk_size,
			Container6Dir<NativeParallelHashSet<int3>> rect_sets)
		{
			//Sets to arrays
			var rect_arrays = await Create(() =>
			{
				return FromResult(new[]
				{
					rect_sets.plus_x.ToNativeArray(Allocator.TempJob),
					rect_sets.mnus_x.ToNativeArray(Allocator.TempJob),
					rect_sets.plus_y.ToNativeArray(Allocator.TempJob),
					rect_sets.mnus_y.ToNativeArray(Allocator.TempJob),
					rect_sets.plus_z.ToNativeArray(Allocator.TempJob),
					rect_sets.mnus_z.ToNativeArray(Allocator.TempJob)
				});
			});

			//Arrays to buffers
			var rect_buffers = await Create(() =>
			{
				return FromResult(rect_arrays.Select(array =>
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
				}).ToArray());
			});

			//Prefix sum
			var rect_array_start_lens = new int2[6];
			int rect_count = 0;
			for (int i = 0; i < 6; i++)
			{
				var cur_len = rect_arrays[i].Length;
				rect_array_start_lens[i] = new(rect_count, cur_len);
				rect_count += cur_len;
			}

			//Init mesh
			var unity_mesh = new Mesh();
			int ib_len = rect_count * 6;
			int vb_len = rect_count * 4;
			unity_mesh.SetIndexBufferParams(ib_len, IndexFormat.UInt32);
			unity_mesh.SetVertexBufferParams(vb_len,
				new VertexAttributeDescriptor(
					VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
				new VertexAttributeDescriptor(
					VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2));
			unity_mesh.indexBufferTarget |= GraphicsBuffer.Target.Structured;
			unity_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Structured;

			GraphicsBuffer vertex_buffer = unity_mesh.GetVertexBuffer(0);

			//Write to ib
			await Create(() =>
			{
				unity_mesh.SetIndexBufferData(index_array.GetSubArray(0, ib_len), 0, 0, ib_len, FAST_SET_FLAG);
				return CompletedTask;
			});

			//Gen vb
			GenRectVB.Plan6Dir(chunk_size, vertex_buffer, rect_array_start_lens, rect_buffers);

			//SetSubMesh
			await Create(() =>
			{
				unity_mesh.subMeshCount = 1;
				unity_mesh.SetSubMesh(0, new(0, rect_count * 6), FAST_SET_FLAG);
				return CompletedTask;
			});

			//Set bound
			unity_mesh.bounds = new((chunk_size / 2).v(), chunk_size.v());

			vertex_buffer.Release();

			return unity_mesh;
		}
	}
}