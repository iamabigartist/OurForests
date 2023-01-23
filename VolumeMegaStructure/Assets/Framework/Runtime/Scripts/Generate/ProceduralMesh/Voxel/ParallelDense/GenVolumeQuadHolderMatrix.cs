using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct GenVolumeQuadHolderMatrix : IJobParallelFor
	{
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public DataMatrix<bool3> volume_quad_holder_matrix;
		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_inside_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_inside_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			volume_quad_holder_matrix[volume_i] = new()
			{
				x = cur_block_inside != x_forward_inside,
				y = cur_block_inside != y_forward_inside,
				z = cur_block_inside != z_forward_inside
			};

		}
	}
}