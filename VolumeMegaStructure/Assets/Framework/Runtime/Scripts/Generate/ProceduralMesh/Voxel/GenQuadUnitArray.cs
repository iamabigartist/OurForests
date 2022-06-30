using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.DataDefinition.DataUnit;
using VolumeMegaStructure.Util;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel
{

	[BurstCompile(DisableSafetyChecks = true)]
	public struct GenQuadUnitArray : IJobParallelFor
	{
		[ReadOnly] public DataMatrix<VolumeUnit> volume_matrix;
		[ReadOnly] public NativeArray<QuadMark> /*.ReadOnly*/ quad_mark_array;
		[WriteOnly] public NativeArray<float> quad_unit_array;

		public void Execute(int quad_i)
		{
			var cur_quad_mark = quad_mark_array[quad_i];
			var (volume_i, dir) = cur_quad_mark;
			var (x, y, z) = volume_matrix.PositionByIndex(volume_i);
			var cur_block_id = volume_matrix[volume_i].block_id;
			float a1, a2, b1, b2, c, forward_block_id, texture_id;
			switch (dir / 2)
			{
				case 0: //x
					{
						a1 = y;
						a2 = y;
						b1 = z;
						b2 = z;
						c = x;
						forward_block_id = volume_matrix[x + 1, y, z].block_id;
						break;
					}
				case 1: //y
					{
						a1 = x;
						a2 = x;
						b1 = z;
						b2 = z;
						c = y;
						forward_block_id = volume_matrix[x, y + 1, z].block_id;
						break;
					}
				case 2: //z
					{
						a1 = x;
						a2 = x;
						b1 = y;
						b2 = y;
						c = z;
						forward_block_id = volume_matrix[x, y, z + 1].block_id;
						break;
					}
				default:
					{
						Debug.Log("dir is not valid");
						throw new("dir is not valid");
					}
			}
			a1 -= 0.5f;
			a2 += 0.5f;
			b1 -= 0.5f;
			b2 += 0.5f;
			c += 0.5f;

			switch (dir % 2)
			{
				case 0:
					{
						texture_id = cur_block_id;
						break;
					}
				case 1:
					{
						texture_id = forward_block_id;
						break;
					}
				default:
					{
						Debug.Log("dir is not valid");
						throw new("dir is not valid");
					}
			}

			int quad_unit_start_index = quad_i * 7;
			quad_unit_array[quad_unit_start_index + 0] = a1;
			quad_unit_array[quad_unit_start_index + 1] = a2;
			quad_unit_array[quad_unit_start_index + 2] = b1;
			quad_unit_array[quad_unit_start_index + 3] = b2;
			quad_unit_array[quad_unit_start_index + 4] = c;
			quad_unit_array[quad_unit_start_index + 5] = dir;
			quad_unit_array[quad_unit_start_index + 6] = texture_id;
		}
	}
}