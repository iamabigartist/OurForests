using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
using VolumeMegaStructure.Util.JobSystem;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	public struct VolumeMatrixScan : IJobParallelFor
	{

	#region Table

		/// <summary>
		///     <para>不透明度小的更加透明,空气不透明度小于固体，范围从0到1.</para>
		///     <para>当遇到交界的时候，只有不透明度更大的需要被渲染出来</para>
		///     <para>但是这就需要后处理等当时渲染水下了，也就是说介质之中往外看的效果</para>
		/// </summary>
		[ReadOnly] public NativeArray<int> id_to_opacity_table;

	#endregion

	#region Data

		[ReadOnly] public Indexer3D indexer;
		[NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<VolumeUnit> volume_matrix;
		[NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<int> mark_matrix;
		NativeCounter quad_counter;

	#endregion

		public void Execute(int volume_i)
		{
			var (x, y, z) = indexer[volume_i];
			var (cur_block_id, cur_rotation_index) = volume_matrix[volume_i];
			var cur_opacity = id_to_opacity_table[cur_block_id];

			if (indexer.IsEdge(x, y, z))
			{
				return;
			}

			var (x_forward_block_id, x_forward_rotation_index) = volume_matrix[indexer[x + 1, y, z]];
			var (y_forward_block_id, y_forward_rotation_index) = volume_matrix[indexer[x, y + 1, z]];
			var (z_forward_block_id, z_forward_rotation_index) = volume_matrix[indexer[x, y, z + 1]];

			var x_forward_opacity = id_to_opacity_table[x_forward_block_id];
			var y_forward_opacity = id_to_opacity_table[y_forward_block_id];
			var z_forward_opacity = id_to_opacity_table[z_forward_block_id];

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