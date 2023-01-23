using Unity.Burst;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.SequentialDense
{
	public struct Container6Dir<T>
	{
		[NoAlias] public T plus_x;
		[NoAlias] public T mnus_x;
		[NoAlias] public T plus_y;
		[NoAlias] public T mnus_y;
		[NoAlias] public T plus_z;
		[NoAlias] public T mnus_z;
	}
}