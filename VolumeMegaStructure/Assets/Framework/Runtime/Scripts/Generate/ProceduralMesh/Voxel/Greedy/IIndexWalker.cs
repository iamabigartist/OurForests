using PrototypePackages.JobUtils;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Greedy
{
	public interface IIndexWalker
	{
		void Walk_In_Line(Index3D i_3d, int cur_i, int step, out int to_i);
		void Walk_Across_Line(Index3D i_3d, int cur_i, int step, out int to_i);
	}
}