using System.Runtime.CompilerServices;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense.Greedy
{
	public interface IIndexWalker
	{
		void Walk(ref int x, ref int y, ref int z, int step);
	}

	public struct X_Quad_Walker : IIndexWalker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Walk(ref int x, ref int y, ref int z, int step) { y += step; }
	}
	public struct Y_Quad_Walker : IIndexWalker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Walk(ref int x, ref int y, ref int z, int step) { x += step; }
	}
	public struct Z_Quad_Walker : IIndexWalker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Walk(ref int x, ref int y, ref int z, int step) { x += step; }
	}

	public struct X_Line_Walker : IIndexWalker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Walk(ref int x, ref int y, ref int z, int step) { z += step; }
	}
	public struct Y_Line_Walker : IIndexWalker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Walk(ref int x, ref int y, ref int z, int step) { z += step; }
	}
	public struct Z_Line_Walker : IIndexWalker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Walk(ref int x, ref int y, ref int z, int step) { y += step; }
	}
}