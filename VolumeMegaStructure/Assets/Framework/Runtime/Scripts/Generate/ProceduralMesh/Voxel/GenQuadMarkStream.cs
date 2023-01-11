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
	public struct GenQuadMarkStream : IJobFor
	{
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeStream.Writer quad_mark_stream;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void CreateSurfaceMark(int volume_i, int axis, bool cur_block_inside)
		{
			int cur_quad_dir = 2 * axis + (cur_block_inside ? 0 : 1);
			QuadMark cur_quad_mark = new(volume_i, cur_quad_dir);
			quad_mark_stream.Write(cur_quad_mark);
		}

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			quad_mark_stream.BeginForEachIndex(volume_i);

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

			quad_mark_stream.EndForEachIndex();

		}

		public static JobHandle ScheduleParallel(
			DataMatrix<bool> volume_inside_matrix,
			out NativeStream quad_mark_stream,
			JobHandle deps = default)
		{
			quad_mark_stream = new(volume_inside_matrix.Count, Allocator.TempJob);
			var job = new GenQuadMarkStream()
			{
				volume_inside_matrix = volume_inside_matrix,
				quad_mark_stream = quad_mark_stream.AsWriter()
			};
			return job.ScheduleParallel(volume_inside_matrix.Count, 1024, deps);
		}
	}
}