using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	public struct VolumeMatrixEmptyCheckInside : IJobParallelFor
	{
		[NativeDisableParallelForRestriction] [ReadOnly] public DataMatrix<ushort> volume_matrix;
		[NativeDisableParallelForRestriction] [WriteOnly] public DataMatrix<bool> volume_inside_matrix;

		public void Execute(int volume_i)
		{
			volume_inside_matrix[volume_i] = volume_matrix[volume_i] != 0;
		}
	}
}