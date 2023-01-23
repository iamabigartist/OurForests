using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenQuadMarkArray : IJobFor
	{
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[ReadOnly] public DataMatrix<bool3> volume_quad_holder_matrix;
		[ReadOnly] public DataMatrix<int> volume_quads_start_index_matrix;
		[WriteOnly] public NativeArray<QuadMark> quad_mark_array;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void CreateSurfaceMark(int mark_index, int volume_i, int axis, bool cur_block_inside)
		{
			int cur_quad_dir = 2 * axis + (cur_block_inside ? 0 : 1);
			QuadMark cur_quad_mark = new(volume_i, cur_quad_dir);
			quad_mark_array[mark_index] = cur_quad_mark;
		}

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			var quad_holder = volume_quad_holder_matrix[volume_i];
			var quads_start_index = volume_quads_start_index_matrix[volume_i];
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }

			int cur_quad_index = quads_start_index;
			if (quad_holder.x)
			{
				CreateSurfaceMark(cur_quad_index, volume_i, 0, cur_block_inside);
				cur_quad_index++;
			}

			if (quad_holder.y)
			{
				CreateSurfaceMark(cur_quad_index, volume_i, 1, cur_block_inside);
				cur_quad_index++;
			}

			if (quad_holder.z)
			{
				CreateSurfaceMark(cur_quad_index, volume_i, 2, cur_block_inside);
				cur_quad_index++;
			}
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<bool> volume_inside_matrix,
			DataMatrix<bool3> volume_quad_holder_matrix,
			DataMatrix<int> volume_quads_start_index_matrix,
			NativeArray<QuadMark> quad_mark_array,
			JobHandle deps = default)
		{
			var job = new GenQuadMarkArray()
			{
				volume_inside_matrix = volume_inside_matrix,
				volume_quad_holder_matrix = volume_quad_holder_matrix,
				volume_quads_start_index_matrix = volume_quads_start_index_matrix,
				quad_mark_array = quad_mark_array
			};
			return job.ScheduleParallel(volume_inside_matrix.Count, 1, deps);
		}
	}
}