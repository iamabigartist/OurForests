using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{


	/// <summary>
	///     在扫描VolumeMatrix之前，是不知道一共有多少个quad需要生成的，而最多可能有3*n*n*(n+1)个quad，这个数量很大，因此使用冗余的数组作为最终quad结果的存放内存很不划算。然而在扫描之后，使用NativeList积累所有quad，最终可以总结出quad的数量，再以此数量分配后续各种数组的长度。
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true)]
	public struct GenQuadMarkList : IJobParallelFor
	{
		[ReadOnly] public DataMatrix<VolumeUnit> volume_matrix;
		[ReadOnly] public DataMatrix<bool> volume_inside_matrix;
		[WriteOnly] public NativeList<QuadMark>.ParallelWriter quad_mark_list;

		void CreateSurfaceMark(int volume_i, int axis, bool cur_block_inside)
		{
			int cur_quad_dir = 2 * axis + (cur_block_inside ? 0 : 1);
			QuadMark cur_quad_mark = new(volume_i, cur_quad_dir);
			quad_mark_list.AddNoResize(cur_quad_mark);
		}

		public void Execute(int volume_i)
		{
			var (x, y, z) = volume_matrix.PositionByIndex(volume_i);
			var cur_block_inside = volume_inside_matrix[volume_i];
			if (volume_matrix.IsPositiveEdge(x, y, z)) { return; }

			var x_forward_inside = volume_inside_matrix[x + 1, y, z];
			var y_forward_inside = volume_inside_matrix[x, y + 1, z];
			var z_forward_inside = volume_inside_matrix[x, y, z + 1];

			if (cur_block_inside != x_forward_inside)
			{
				CreateSurfaceMark(volume_i, 0, cur_block_inside);
			}

			if (cur_block_inside != y_forward_inside)
			{
				CreateSurfaceMark(volume_i, 1, cur_block_inside);
			}

			if (cur_block_inside != z_forward_inside)
			{
				CreateSurfaceMark(volume_i, 2, cur_block_inside);
			}
		}
	}
}