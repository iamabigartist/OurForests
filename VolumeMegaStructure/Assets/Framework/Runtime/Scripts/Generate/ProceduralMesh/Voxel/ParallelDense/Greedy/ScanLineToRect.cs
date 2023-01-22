using PrototypePackages.JobUtils.Template;
using PrototypePackages.MathematicsUtils.Index;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.Util;
using static PrototypePackages.JobUtils.Template.IPlanFor;
using static VolumeMegaStructure.Util.JobSystem.ScheduleUtils;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Greedy
{
	public static class ScanLineToRect
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		public struct ScanLineToRectJob<TIndexWalker> : IJobFor, IPlanFor
			where TIndexWalker : struct, IIndexWalker
		{
			public int length => line_array.Length;
			public int batch => GetBatchSize_WorkerThreadCount(length, 3);

			public TIndexWalker walker;
			public Index3D c;
			[NoAlias] [ReadOnly] public NativeArray<int3> line_array;
			[NoAlias] [ReadOnly] public NativeParallelHashSet<int3>.ReadOnly line_set;
			[NoAlias] [WriteOnly] public NativeQueue<int3>.ParallelWriter rect_queue;
			public void Execute(int i_line)
			{
				var (block_id, start_pos, end_pos) = line_array[i_line];
				c.To3D(start_pos, out var start_x, out var start_y, out var start_z);
				c.To3D(end_pos, out var end_x, out var end_y, out var end_z);

				var (beside_start_x, beside_start_y, beside_start_z) = (start_x, start_y, start_z);
				walker.Walk(ref beside_start_x, ref beside_start_y, ref beside_start_z, -1);
				c.To1D(beside_start_x, beside_start_y, beside_start_z, out var beside_start_pos);

				var (beside_end_x, beside_end_y, beside_end_z) = (end_x, end_y, end_z);
				walker.Walk(ref beside_end_x, ref beside_end_y, ref beside_end_z, -1);
				c.To1D(beside_end_x, beside_end_y, beside_end_z, out var beside_end_pos);

				if (line_set.Contains(new(block_id, beside_start_pos, beside_end_pos))) { return; }

				(beside_start_x, beside_start_y, beside_start_z) = (start_x, start_y, start_z);
				(beside_end_x, beside_end_y, beside_end_z) = (end_x, end_y, end_z);
				var rect_end_pos = end_pos;
				while (true)
				{
					walker.Walk(ref beside_start_x, ref beside_start_y, ref beside_start_z, 1);
					c.To1D(beside_start_x, beside_start_y, beside_start_z, out beside_start_pos);
					walker.Walk(ref beside_end_x, ref beside_end_y, ref beside_end_z, 1);
					c.To1D(beside_end_x, beside_end_y, beside_end_z, out beside_end_pos);
					if (!line_set.Contains(new(block_id, beside_start_pos, beside_end_pos))) { break; }
					rect_end_pos = beside_end_pos;
				}
				rect_queue.Enqueue(new(block_id, start_pos, rect_end_pos));
			}

			public ScanLineToRectJob(
				int3 matrix_size,
				NativeArray<int3> line_array,
				NativeParallelHashSet<int3> line_set,
				out NativeQueue<int3> rect_queue)
			{
				rect_queue = new(Allocator.TempJob);
				walker = new();
				c = new(matrix_size);
				this.line_array = line_array;
				this.line_set = line_set.AsReadOnly();
				this.rect_queue = rect_queue.AsParallelWriter();
			}
		}

		public static JobHandle Plan6Dir(
			int3 size,
			NativeArray<int3>[] line_arrays,
			NativeParallelHashSet<int3>[] line_sets,
			out NativeQueue<int3>[] rect_queues)
		{
			rect_queues = new NativeQueue<int3>[6];
			var jhs = new JobHandle[6];
			jhs[0] = Plan(new ScanLineToRectJob<X_Line_Walker>(size, line_arrays[0], line_sets[0], out rect_queues[0]));
			jhs[1] = Plan(new ScanLineToRectJob<X_Line_Walker>(size, line_arrays[1], line_sets[1], out rect_queues[1]));
			jhs[2] = Plan(new ScanLineToRectJob<Y_Line_Walker>(size, line_arrays[2], line_sets[2], out rect_queues[2]));
			jhs[3] = Plan(new ScanLineToRectJob<Y_Line_Walker>(size, line_arrays[3], line_sets[3], out rect_queues[3]));
			jhs[4] = Plan(new ScanLineToRectJob<Z_Line_Walker>(size, line_arrays[4], line_sets[4], out rect_queues[4]));
			jhs[5] = Plan(new ScanLineToRectJob<Z_Line_Walker>(size, line_arrays[5], line_sets[5], out rect_queues[5]));
			return jhs.Combine();
		}
	}

}