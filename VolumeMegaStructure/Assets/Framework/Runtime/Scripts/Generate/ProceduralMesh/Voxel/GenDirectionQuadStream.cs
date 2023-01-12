using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	/// <summary>
	///     统计所有quad的必要信息，以随机顺序组成一个列表，供后面的数组赋值操作时候参考位置。
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenDirectionQuadStream : IJobFor
	{
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeStream.Writer stream_x;
		[WriteOnly] public NativeStream.Writer stream_x_minus;
		[WriteOnly] public NativeStream.Writer stream_y;
		[WriteOnly] public NativeStream.Writer stream_y_minus;
		[WriteOnly] public NativeStream.Writer stream_z;
		[WriteOnly] public NativeStream.Writer stream_z_minus;

		void WriteQuadToStream(int i, ref NativeStream.Writer stream)
		{
			stream.BeginForEachIndex(i);
			stream.Write(i);
			stream.EndForEachIndex();
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
				if (cur_block_inside) { WriteQuadToStream(volume_i, ref stream_x); }
				else { WriteQuadToStream(volume_i, ref stream_x_minus); }
			}

			if (cur_block_inside != y_forward_inside)
			{
				if (cur_block_inside) { WriteQuadToStream(volume_i, ref stream_y); }
				else { WriteQuadToStream(volume_i, ref stream_y_minus); }
			}

			if (cur_block_inside != z_forward_inside)
			{
				if (cur_block_inside) { WriteQuadToStream(volume_i, ref stream_z); }
				else { WriteQuadToStream(volume_i, ref stream_z_minus); }
			}
		}

		public static JobHandle ScheduleParallel(
			DataMatrix<bool> volume_inside_matrix,
			out NativeStream stream_x,
			out NativeStream stream_x_minus,
			out NativeStream stream_y,
			out NativeStream stream_y_minus,
			out NativeStream stream_z,
			out NativeStream stream_z_minus,
			JobHandle deps = default)
		{
			int volume_count = volume_inside_matrix.Count;
			stream_x = new(volume_count, Allocator.TempJob);
			stream_x_minus = new(volume_count, Allocator.TempJob);
			stream_y = new(volume_count, Allocator.TempJob);
			stream_y_minus = new(volume_count, Allocator.TempJob);
			stream_z = new(volume_count, Allocator.TempJob);
			stream_z_minus = new(volume_count, Allocator.TempJob);
			var job = new GenDirectionQuadStream()
			{
				volume_inside_matrix = volume_inside_matrix,
				stream_x = stream_x.AsWriter(),
				stream_x_minus = stream_x_minus.AsWriter(),
				stream_y = stream_y.AsWriter(),
				stream_y_minus = stream_y_minus.AsWriter(),
				stream_z = stream_z.AsWriter(),
				stream_z_minus = stream_z_minus.AsWriter()

			};
			return job.ScheduleParallel(volume_inside_matrix.Count, 1024, deps);
		}
	}
}