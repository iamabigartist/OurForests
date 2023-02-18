using System;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree
{
	public interface ITreeUnit<TUnitInfo>
		where TUnitInfo : struct, IEquatable<TUnitInfo>
	{
		void Deconstruct(out int size_level, out TUnitInfo info);
	}
}