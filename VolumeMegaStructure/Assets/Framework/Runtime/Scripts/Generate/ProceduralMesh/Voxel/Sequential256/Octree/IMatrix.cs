using System;
using Unity.Mathematics;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public interface IMatrix<TUnitInfo> : IContainer
		where TUnitInfo : struct, IEquatable<TUnitInfo>
	{
		void Get(in int3 position, out TUnitInfo info);
		void Set(in int3 position, in TUnitInfo info);
	}
}