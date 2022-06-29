using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	[BurstCompile(DisableSafetyChecks = true)]
	public struct VolumeMatrixEmptyCheckInside : IJobParallelFor
	{
		[NativeDisableParallelForRestriction] [ReadOnly] public DataMatrix<VolumeUnit> volume_matrix;
		[NativeDisableParallelForRestriction] [ReadOnly] public DataMatrix<bool> volume_inside_matrix;

		public void Execute(int volume_i)
		{
			volume_inside_matrix[volume_i] = volume_matrix[volume_i].block_id != 0;
		}
	}
}