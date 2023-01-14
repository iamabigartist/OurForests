using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.Util;
using VolumeMegaStructure.Util.JobSystem;
using static PrototypePackages.JobUtils.IndexUtil;
using static VolumeMegaStructure.Util.JobSystem.IForPlan;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Greedy
{
	public static class ScanQuadToLine
	{
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		public struct ScanQuadToLineJob<TIndexWalker> : IJobFor, IForPlan
			where TIndexWalker : struct, IIndexWalker
		{
			public int length => quad_array.Length;
			public int batch => 1024;
			public TIndexWalker walker;
			public Index3D c;
			[ReadOnly] public NativeArray<int2> quad_array;
			[ReadOnly] public NativeHashSet<int2> quad_set;
			[WriteOnly] public NativeQueue<int3>.ParallelWriter line_queue;
			public void Execute(int i_quad)
			{
				var (block_id, start_pos) = quad_array[i_quad];
				c.To3D(start_pos, out var x, out var y, out var z);
				var (beside_x, beside_y, beside_z) = (x, y, z);
				walker.Walk(ref beside_x, ref beside_y, ref beside_z, -1);
				var beside_pos = c[beside_x, beside_y, beside_z];
				if (quad_set.Contains(new(block_id, beside_pos))) { return; }
				(beside_x, beside_y, beside_z) = (x, y, z);
				var end_pos = start_pos;
				while (true)
				{
					walker.Walk(ref beside_x, ref beside_y, ref beside_z, 1);
					beside_pos = c[beside_x, beside_y, beside_z];
					if (!quad_set.Contains(new(block_id, beside_pos))) { break; }
					end_pos = beside_pos;
				}
				line_queue.Enqueue(new(block_id, start_pos, end_pos));
			}

			public ScanQuadToLineJob(
				int3 matrix_size,
				NativeArray<int2> quad_array,
				NativeHashSet<int2> quad_set,
				out NativeQueue<int3> line_queue)
			{
				line_queue = new(Allocator.TempJob);
				walker = new();
				c = new(matrix_size);
				this.quad_array = quad_array;
				this.quad_set = quad_set;
				this.line_queue = line_queue.AsParallelWriter();
			}
		}

		public static JobHandle Plan6Dir(
			int3 size,
			NativeArray<int2>[] quad_arrays,
			NativeHashSet<int2>[] quad_sets,
			out NativeQueue<int3>[] line_queues)
		{
			line_queues = new NativeQueue<int3>[6];
			var jhs = new JobHandle[6];
			jhs[0] = Plan(new ScanQuadToLineJob<X_Line_Walker>(size, quad_arrays[0], quad_sets[0], out line_queues[0]));
			jhs[1] = Plan(new ScanQuadToLineJob<X_Line_Walker>(size, quad_arrays[1], quad_sets[1], out line_queues[1]));
			jhs[2] = Plan(new ScanQuadToLineJob<Y_Line_Walker>(size, quad_arrays[2], quad_sets[2], out line_queues[2]));
			jhs[3] = Plan(new ScanQuadToLineJob<Y_Line_Walker>(size, quad_arrays[3], quad_sets[3], out line_queues[3]));
			jhs[4] = Plan(new ScanQuadToLineJob<Z_Line_Walker>(size, quad_arrays[4], quad_sets[4], out line_queues[4]));
			jhs[5] = Plan(new ScanQuadToLineJob<Z_Line_Walker>(size, quad_arrays[5], quad_sets[5], out line_queues[5]));
			return jhs.Combine();
		}
	}

}