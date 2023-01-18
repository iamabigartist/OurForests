﻿using System;
using System.Threading.Tasks;
using UnityEngine;
using VolumeMegaStructure.DataDefinition.Container;
using VolumeMegaStructure.Generate.Volume.Noise;
using Random = UnityEngine.Random;
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

		public static void GenerateCoherentNoise01<T>(this DataMatrix<T> matrix, float ratio_0, T element_0, T element_1, string seed)
			where T : struct
		{
			var noisier = new SimplexNoiseGenerator(seed);

			for (int z = 0; z < matrix.size.z; z++)
			{
				for (int y = 0; y < matrix.size.y; y++)
				{
					for (int x = 0; x < matrix.size.x; x++)
					{
						// Debug.Log((noisier.coherentNoise(x, y, z) + 1) / 2);
						matrix[x, y, z] = (noisier.coherentNoise(x, y, z) + 1f) / 2f < (noisier.coherentNoise(20, 20, 20) + 1f) / 2f ? element_0 : element_1;
						// matrix[x, y, z] = (dynamic)(noisier.coherentNoise( x, y, z ) + 1) * 5;
					}
				}
			}
		}

		public static void GenerateSphere01<T>(this DataMatrix<T> matrix, T inside_value, T outside_value, float radius, Vector3Int mid_point)
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

		[Serializable]
		public struct GenBlockParams
		{
			public ushort block_id;
			public int cube_height;
			public float noise_height_scale;
			public float noise_scale;
			public Vector2 noise_offset;
			public GenBlockParams(ushort block_id, int cube_height, float noise_height_scale, float noise_scale, Vector2 noise_offset)
			{
				this.block_id = block_id;
				this.cube_height = cube_height;
				this.noise_height_scale = noise_height_scale;
				this.noise_scale = noise_scale;
				this.noise_offset = noise_offset;
			}

			public Vector2 GetSamplePosition(Vector2 position)
			{
				return position * noise_scale + noise_offset;
			}

			public float GetHeight(float noise)
			{
				var scaled_noise = noise * noise_height_scale;
				// float height = (int)scaled_noise / cube_height * cube_height;
				float height = scaled_noise;
				return height;
			}
		}

		[Serializable]
		public struct GenerateGrassSnowTerrainParams
		{
			public GenBlockParams stone_params;
			public GenBlockParams soil_params;
			public ushort grass_id;
			public ushort snow_id;
			public float snow_min_altitude;
			public float grass_max_altitude;
			public float noise_scale;
			public Vector2 noise_offset;
		}

		public static void GenerateGrassSnowTerrain(
			this DataMatrix<ushort> matrix,
			GenerateGrassSnowTerrainParams parameters
		)
		{
			matrix.GenerateGrassSnowTerrain(
				parameters.stone_params,
				parameters.soil_params,
				parameters.grass_id,
				parameters.snow_id,
				parameters.snow_min_altitude,
				parameters.grass_max_altitude,
				parameters.noise_scale,
				parameters.noise_offset);
		}

		public static void GenerateGrassSnowTerrain(
			this DataMatrix<ushort> matrix,
			GenBlockParams stone_params,
			GenBlockParams soil_params,
			ushort grass_id,
			ushort snow_id,
			float snow_min_altitude,
			float grass_max_altitude,
			float noise_scale,
			Vector2 noise_offset
		)
		{
			Vector2 TransformSamplePosition(Vector2 position)
			{
				return position * noise_scale + noise_offset;
			}

			float GetHeight(GenBlockParams block_params, Vector2 position)
			{
				var noise_position = TransformSamplePosition(block_params.GetSamplePosition(position));
				float height = block_params.GetHeight(Mathf.PerlinNoise(noise_position.x, noise_position.y));
				return height;
			}

			Parallel.For(0, matrix.size.z, z =>
			{
				for (int x = 0; x < matrix.size.x; x++)
				{
					var position = new Vector2(x, z);
					float stone_height = GetHeight(stone_params, position);
					float soil_height = GetHeight(soil_params, position);

					for (int y = 0; y < matrix.size.y; y++)
					{
						if (y <= stone_height)
						{
							matrix[x, y, z] = stone_params.block_id;
						}
						else if (y <= stone_height + soil_height)
						{
							matrix[x, y, z] = soil_params.block_id;
						}
						else if (y <= stone_height + soil_height + 1)
						{
							if (y <= grass_max_altitude)
							{
								matrix[x, y, z] = grass_id;
							}
							else if (y <= snow_min_altitude)
							{
								matrix[x, y, z] = 0;
							}
							else
							{
								matrix[x, y, z] = snow_id;
							}
						}
						else
						{
							matrix[x, y, z] = 0;
						}
					}
				}
			});
		}

	#endregion
		//TODO classify methods into different class//

	}
}