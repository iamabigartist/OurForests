using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	public class MeshGeneration
	{
		public static async UniTask<Mesh> GenMeshAsync(
			NativeArray<int> index_array,
			int3 chunk_size,
			Container6Dir<NativeHashSet<int3>> rect_sets)
		{
			//Sets to arrays
			var rect_arrays = await UniTask.Create(() =>
			{
				return UniTask.FromResult(new[]
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
			var rect_buffers = await UniTask.Create(() =>
			{
				return UniTask.FromResult(rect_arrays.Select(array =>
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
			GraphicsBuffer index_buffer = unity_mesh.GetIndexBuffer();
			GraphicsBuffer vertex_buffer = unity_mesh.GetVertexBuffer(0);

			//Write to ib
			await UniTask.Create(() =>
			{
				var ib_native_array = index_buffer.LockBufferForWrite<int>(0, ib_len);
				index_array.Slice(0, ib_len).CopyTo(ib_native_array);
				index_buffer.UnlockBufferAfterWrite<int>(ib_len);
				return UniTask.CompletedTask;
			});

			//Gen vb
			GenRectVB.Plan6Dir(chunk_size, vertex_buffer, rect_array_start_lens, rect_buffers);

			return unity_mesh;
		}
	}
}