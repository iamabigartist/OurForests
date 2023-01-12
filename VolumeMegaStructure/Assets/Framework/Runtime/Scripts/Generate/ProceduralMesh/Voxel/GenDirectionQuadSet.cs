using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     统计所有quad的必要信息，以随机顺序组成一个列表，供后面的数组赋值操作时候参考位置。
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenDirectionQuadSet : IJobFor
	{
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeHashSet<int>.ParallelWriter stream_x_minus;
		[WriteOnly] public NativeHashSet<int>.ParallelWriter stream_y;
		[WriteOnly] public NativeHashSet<int>.ParallelWriter stream_y_minus;
		[WriteOnly] public NativeHashSet<int>.ParallelWriter stream_z;
		[WriteOnly] public NativeHashSet<int>.ParallelWriter stream_z_minus;
		[WriteOnly] public NativeHashSet<int>.ParallelWriter stream_x;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void WriteQuadToStream(int i, ref NativeHashSet<int>.ParallelWriter stream)
		{
			stream.Add(i);
		}

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_block_inside != x_forward_inside)
			{
				if (cur_block_inside) { stream_x.Add(volume_i); }
				else { stream_x_minus.Add(volume_i); }
			}

			if (cur_block_inside != y_forward_inside)
			{
				if (cur_block_inside) { stream_y.Add(volume_i); }
				else { stream_y_minus.Add(volume_i); }
			}

			if (cur_block_inside != z_forward_inside)
			{
				if (cur_block_inside) { stream_z.Add(volume_i); }
				else { stream_z_minus.Add(volume_i); }
			}
		}

		static int GetBatchSize_SquareThreadSeparate(int total)
		{
			float pow = math.pow(JobsUtility.MaxJobThreadCount, 2);

			return (int)math.ceil(total / pow);
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<bool> volume_inside_matrix,
			out NativeHashSet<int> stream_x,
			out NativeHashSet<int> stream_x_minus,
			out NativeHashSet<int> stream_y,
			out NativeHashSet<int> stream_y_minus,
			out NativeHashSet<int> stream_z,
			out NativeHashSet<int> stream_z_minus,
			JobHandle deps = default)
		{
			int volume_count = volume_inside_matrix.Count;
			stream_x = new(1, Allocator.TempJob);
			stream_x_minus = new(1, Allocator.TempJob);
			stream_y = new(1, Allocator.TempJob);
			stream_y_minus = new(1, Allocator.TempJob);
			stream_z = new(1, Allocator.TempJob);
			stream_z_minus = new(1, Allocator.TempJob);
			var job = new GenDirectionQuadSet()
			{
				volume_inside_matrix = volume_inside_matrix,
				stream_x = stream_x.AsParallelWriter(),
				stream_x_minus = stream_x_minus.AsParallelWriter(),
				stream_y = stream_y.AsParallelWriter(),
				stream_y_minus = stream_y_minus.AsParallelWriter(),
				stream_z = stream_z.AsParallelWriter(),
				stream_z_minus = stream_z_minus.AsParallelWriter()
			};
			return job.ScheduleParallel(volume_count,
				GetBatchSize_SquareThreadSeparate(volume_count), deps);
		}
	}
}