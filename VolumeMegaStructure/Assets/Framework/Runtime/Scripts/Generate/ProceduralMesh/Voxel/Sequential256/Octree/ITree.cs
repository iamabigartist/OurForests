using System;
using Unity.Mathematics;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public interface ITree<TTreeUnit, TUnitInfo> : IContainer
		where TTreeUnit : ITreeUnit<TUnitInfo>
		where TUnitInfo : struct, IEquatable<TUnitInfo>
	{
		bool TryGetRegionByAnchor(in int3 anchor, out TTreeUnit unit);
		void Add(in int3 anchor, in int size_level, in TUnitInfo info);
		void Remove(in int3 anchor);
	}
}