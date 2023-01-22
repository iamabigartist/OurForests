using PrototypePackages.MathematicsUtils.Index;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using static UnityEngine.Mesh;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
	public static class GenMesh
	{
		public struct GenMeshJob : IJob
		{
			int3 chunk_size;
			Index3D coordinate;
			[ReadOnly] NativeArray<ushort> chunk;
			[ReadOnly] NativeArray<ushort> chunk_neighbor_x;
			[ReadOnly] NativeArray<ushort> chunk_neighbor_y;
			[ReadOnly] NativeArray<ushort> chunk_neighbor_z;
			MeshData mesh;

			public void GenQuadUnit()
			{
				UnsafeList<NativeArray<int>> aa;
				UnsafeAppendBuffer buffer1;
				for (int z = 0; z < chunk_size.z - 1; z++)
				{
					for (int y = 0; y < chunk_size.y; y++)
					{
						for (int x = 0; x < chunk_size.x; x++) {}
					}
				}
			}
			public void Execute()
			{
				//1. Gen QuadUnit

				//Greedy QuadUnit
				//Gen QuadMesh
			}
		}
	}
}