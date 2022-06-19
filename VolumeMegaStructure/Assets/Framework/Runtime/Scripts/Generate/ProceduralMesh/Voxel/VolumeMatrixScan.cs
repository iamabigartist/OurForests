using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	public struct VolumeMatrixScan : IJobParallelFor
	{

	#region Table

		[ReadOnly] public NativeArray<bool> IsOpacityById;

	#endregion

	#region Data

		[NativeDisableParallelForRestriction] [ReadOnly] public DataMatrix<VolumeUnit> volume_matrix;
		[NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<int> mark_matrix;
		NativeCounter quad_counter;

	#endregion

		public void Execute(int volume_i)
		{
			var (x, y, z) = indexer[volume_i];
			var cur_block_id = volume_matrix[volume_i].block_id;
			var cur_opacity = id_to_opacity[cur_block_id];

			if (indexer.IsEdge(x, y, z))
			{
				return;
			}

			var x_forward_block_id = volume_matrix[indexer[x + 1, y, z]].block_id;
			var y_forward_block_id = volume_matrix[indexer[x, y + 1, z]].block_id;
			var z_forward_block_id = volume_matrix[indexer[x, y, z + 1]].block_id;

			var x_forward_opacity = id_to_opacity[x_forward_block_id];
			var y_forward_opacity = id_to_opacity[y_forward_block_id];
			var z_forward_opacity = id_to_opacity[z_forward_block_id];

			if (x_forward_opacity != cur_opacity) {}

			if (y_forward_opacity != cur_opacity) {}

			if (z_forward_opacity != cur_opacity) {}
		}

	}

	public class Executor
	{
		public Executor()
		{
			var buffer = new ComputeBuffer(100, 4);
			VolumeMatrixScan volume_matrix_scan = new VolumeMatrixScan();
			volume_matrix_scan.mark_matrix = buffer.BeginWrite<int>(0, 100);
			buffer.EndWrite<int>(1000);
		}
	}
}