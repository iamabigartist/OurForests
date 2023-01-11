using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     统计所有quad的必要信息，以随机顺序组成一个列表，供后面的数组赋值操作时候参考位置。
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenQuadMarkList : IJobFor
	{
		[ReadOnly] public DataMatrix<VolumeUnit> volume_matrix;
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeList<QuadMark>.ParallelWriter quad_mark_list;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void CreateSurfaceMark(int volume_i, int axis, bool cur_block_inside)
		{
			int cur_quad_dir = 2 * axis + (cur_block_inside ? 0 : 1);
			QuadMark cur_quad_mark = new(volume_i, cur_quad_dir);
			quad_mark_list.AddNoResize(cur_quad_mark);
		}

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_block_inside != x_forward_inside)
			{
				CreateSurfaceMark(volume_i, 0, cur_block_inside);
			}

			if (cur_block_inside != y_forward_inside)
			{
				CreateSurfaceMark(volume_i, 1, cur_block_inside);
			}

			if (cur_block_inside != z_forward_inside)
			{
				CreateSurfaceMark(volume_i, 2, cur_block_inside);
			}
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<VolumeUnit> volume_matrix,
			DataMatrix<bool> volume_inside_matrix,
			int max_quad_count,
			out NativeList<QuadMark> quad_mark_list,
			JobHandle deps = default)
		{
			quad_mark_list = new(max_quad_count, Allocator.Persistent);
			var job = new GenQuadMarkList()
			{
				volume_matrix = volume_matrix,
				volume_inside_matrix = volume_inside_matrix,
				quad_mark_list = quad_mark_list.AsParallelWriter()
			};
			return job.ScheduleParallel(volume_matrix.Count, 1, deps);
		}
	}
}