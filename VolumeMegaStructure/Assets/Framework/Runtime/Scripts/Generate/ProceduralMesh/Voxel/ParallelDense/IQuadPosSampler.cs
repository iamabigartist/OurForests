using System.Runtime.CompilerServices;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense
{
	public interface IQuadPosSampler
	{
		void GetQuadPos(ref int x, ref int y, ref int z);
	}
	public struct PlusQuadPosSampler : IQuadPosSampler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetQuadPos(ref int x, ref int y, ref int z) {}
	}

	public struct XMinusQuadPosSampler : IQuadPosSampler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetQuadPos(ref int x, ref int y, ref int z) { x++; }
	}

	public struct YMinusQuadPosSampler : IQuadPosSampler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetQuadPos(ref int x, ref int y, ref int z) { y++; }
	}

	public struct ZMinusQuadPosSampler : IQuadPosSampler
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void GetQuadPos(ref int x, ref int y, ref int z) { z++; }
	}
}