using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Generate.Volume.Noise;
namespace VolumeMegaStructure.Generate.Volume
{
	public static class VolumeMatrixGeneration
	{

	#region QuickTests

		public static void GenerateRandom01<T>(this DataMatrix<T> matrix, float ratio_0, T element_0, T element_1)
			where T : struct
		{
			for (int i = 0; i < matrix.Count; i++)
			{
				matrix[i] = Random.Range(0f, 1f) < ratio_0 ? element_0 : element_1;
			}
		}

		public static void GenerateCoherentNoiseThreshold<T>(this DataMatrix<T> matrix, Vector2 range, string seed)
			where T : struct
		{
			var noisier = new SimplexNoiseGenerator(seed);

			for (int z = 0; z < matrix.size.z; z++)
			{
				for (int y = 0; y < matrix.size.y; y++)
				{
					for (int x = 0; x < matrix.size.x; x++)
					{
						matrix[x, y, z] = (dynamic)
							((noisier.coherentNoise(x, y, z) + 1) / 2 *
								(range.y - range.x) + range.x);
						// matrix[x, y, z] = (dynamic)(noisier.coherentNoise( x, y, z ) + 1) * 5;
					}
				}
			}
		}

		public static void GenerateSphereThreshold<T>(this DataMatrix<T> matrix, T inside_value, T outside_value, float radius, Vector3Int mid_point)
			where T : struct
		{
			for (int z = 0; z < matrix.size.z; z++)
			{
				for (int y = 0; y < matrix.size.y; y++)
				{
					for (int x = 0; x < matrix.size.x; x++)
					{
						float distance = Vector3.Distance(
							new(x, y, z),
							mid_point);
						matrix[x, y, z] =
							distance < radius ? inside_value : outside_value;

					}
				}
			}
		}


		/// <summary>
		///     <para>0 for air.</para>
		///     <para>1 for soil.</para>
		///     <para>2 for stone.</para>
		/// </summary>
		public static void GenerateSimpleTerrain(
			this DataMatrix<int> matrix,
			float stone_height_scale,
			float soil_height_scale,
			Vector2 noise_offset,
			float noise_scale = 1,
			int stone_id = 1,
			int soil_id = 1)
		{

			Vector2 transform(Vector2 position)
			{
				return position * noise_scale + noise_offset;
			}


			for (int z = 0; z < matrix.size.z; z++)
			{
				for (int x = 0; x < matrix.size.x; x++)
				{
					var stone_noise_position = transform(new(x, z));
					float stone_height = Mathf.PerlinNoise(stone_noise_position.x, stone_noise_position.y) * stone_height_scale;
					var soil_noise_position = transform(new(x + 5000, z + 5000));
					float soil_height = Mathf.PerlinNoise(soil_noise_position.x, soil_noise_position.y) * soil_height_scale;

					for (int y = 0; y < matrix.size.y; y++)
					{
						if (y <= stone_height)
						{
							matrix[x, y, z] = stone_id;
						}
						else if (y <= stone_height + soil_height)
						{
							matrix[x, y, z] = soil_id;
						}
						else
						{
							matrix[x, y, z] = 0;
						}
					}

				}
			}

		}

	#endregion
		//TODO classify methods into different class//

	}
}