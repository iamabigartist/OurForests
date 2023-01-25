using System;
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

	public static class Container6DirUtil
	{
		public static void Dispose<T>(this Container6Dir<T> container)
			where T : struct, IDisposable
		{
			container.plus_x.Dispose();
			container.mnus_x.Dispose();
			container.plus_y.Dispose();
			container.mnus_y.Dispose();
			container.plus_z.Dispose();
			container.mnus_z.Dispose();
		}
	}
}