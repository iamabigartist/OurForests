using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Util.JobSystem;
using static PrototypePackages.JobUtils.IndexUtil;
using static VolumeMegaStructure.Util.JobSystem.IForPlan;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	public static class GenQuadUnitArray_New
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		public struct GenQuadIdJob<TQuadPosSampler> : IJobFor, IForPlan
			where TQuadPosSampler : struct, IQuadPosSampler
		{
			public int length => quad_pos_array.Length;
			public int batch => 1024;

			public TQuadPosSampler sampler;
			public Index3D c;
			[ReadOnly] public DataMatrix<ushort> volume_matrix;
			[ReadOnly] public NativeArray<int> quad_pos_array;
			[WriteOnly] public NativeArray<int2> quad_unit_array;
			public void Execute(int i_quad)
			{
				var pos = quad_pos_array[i_quad];
				c.To3D(pos, out var x, out var y, out var z);
				sampler.GetQuadPos(ref x, ref y, ref z);
				quad_unit_array[i_quad] = new(volume_matrix[x, y, z], pos);
			}

			public GenQuadIdJob(
				int3 matrix_size,
				int array_len,
				DataMatrix<ushort> volume_matrix,
				NativeArray<int> quad_pos_array,
				out NativeArray<int2> quad_unit_array)
			{
				quad_unit_array = new(array_len, Allocator.TempJob);
				sampler = new();
				c = new(matrix_size);
				this.volume_matrix = volume_matrix;
				this.quad_pos_array = quad_pos_array;
				this.quad_unit_array = quad_unit_array;
			}
		}

		public static JobHandle Plan6Dir(
			int3 matrix_size, int[] array_lens,
			DataMatrix<ushort> volume_matrix,
			NativeArray<int>[] quad_pos_arrays,
			out NativeArray<int2>[] quad_id_arrays)
		{
			quad_id_arrays = new NativeArray<int2>[6];
			var jhs = new JobHandle[6];
			jhs[0] = Plan(new GenQuadIdJob<PlusQuadPosSampler>(matrix_size, array_lens[0], volume_matrix, quad_pos_arrays[0], out quad_id_arrays[0]));
			jhs[2] = Plan(new GenQuadIdJob<PlusQuadPosSampler>(matrix_size, array_lens[2], volume_matrix, quad_pos_arrays[2], out quad_id_arrays[2]));
			jhs[4] = Plan(new GenQuadIdJob<PlusQuadPosSampler>(matrix_size, array_lens[4], volume_matrix, quad_pos_arrays[4], out quad_id_arrays[4]));
			jhs[1] = Plan(new GenQuadIdJob<XMinusQuadPosSampler>(matrix_size, array_lens[1], volume_matrix, quad_pos_arrays[1], out quad_id_arrays[1]));
			jhs[3] = Plan(new GenQuadIdJob<YMinusQuadPosSampler>(matrix_size, array_lens[3], volume_matrix, quad_pos_arrays[3], out quad_id_arrays[3]));
			jhs[5] = Plan(new GenQuadIdJob<ZMinusQuadPosSampler>(matrix_size, array_lens[5], volume_matrix, quad_pos_arrays[5], out quad_id_arrays[5]));
			return jhs.Combine();
		}
	}
}