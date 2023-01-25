using PrototypePackages.JobUtils.Template;
using PrototypePackages.MathematicsUtils.Index;
using PrototypePackages.MathematicsUtils.Noise;
using PrototypePackages.MathematicsUtils.ValueTransformers;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using VolumeMegaStructure.Util;
using static PrototypePackages.MathematicsUtils.ValueTransformers.Sampler;
using static Unity.Mathematics.noise;
namespace VolumeMegaStructure.Generate.Volume.Terrain
{
	public static class TestTerrainGeneration
	{
		[BurstCompile(OptimizeFor = OptimizeFor.Performance, DisableSafetyChecks = true)]
		public struct GenSimpleMountainTerrainJob : IJob, IPlan
		{
			[NoAlias] ushort grass_id;
			[NoAlias] ushort snow_id;
			[NoAlias] ushort soil_id;
			[NoAlias] ushort stone_id;
			[NoAlias] ushort air_id;
			[NoAlias] int grass_max_altitude;
			[NoAlias] int snow_min_altitude;
			[NoAlias] int2 chunk_pos;
			[NoAlias] sample_transform<float2, op_f2> position_transform;
			[NoAlias] PlaneHeightNoise<PerlinNoise_f2> terrain_altitude_noise;
			[NoAlias] PlaneHeightNoise<PerlinNoise_f2> terrain_texture_noise;
			[NoAlias] Index3D coordinate;
			[NoAlias] [WriteOnly] NativeArray<ushort> chunk;

			public void Execute()
			{
				int3 chunk_size = coordinate.size;
				for (int z = 0; z < chunk_size.z; z++)
				{
					for (int x = 0; x < chunk_size.x; x++)
					{
						float2 cur_pos = new int2(x, z) + chunk_pos;
						position_transform.TransformIn(cur_pos, out var sample_position);
						var test_p = (pnoise(sample_position, 100) + 1f) / 2f;
						terrain_altitude_noise.Sample(sample_position, out var altitude_height);
						terrain_texture_noise.Sample(sample_position, out var texture_height);

						// Debug.Log(
						// 	$"cur_pos: {cur_pos}, sample_position: {sample_position}, altitude_height: {altitude_height}, test_p: {test_p}");

						var soil_height = altitude_height + texture_height;
						var surface_height = altitude_height + texture_height + 1;
						for (int y = 0; y < chunk_size.y; y++)
						{
							coordinate.To1D(x, y, z, out var i);
							if (y <= altitude_height) { chunk[i] = stone_id; }
							else if (y <= soil_height) { chunk[i] = soil_id; }
							else if (y <= surface_height)
							{
								if (y <= grass_max_altitude)
								{
									chunk[i] = grass_id;
								}
								else if (y <= snow_min_altitude)
								{
									chunk[i] = soil_id;
								}
								else
								{
									chunk[i] = snow_id;
								}
							}
							else
							{
								chunk[i] = air_id;
							}
						}
					}
				}
			}

			public GenSimpleMountainTerrainJob(
				ushort grass_id, ushort snow_id, ushort soil_id, ushort stone_id, ushort air_id,
				int grass_max_altitude, int snow_min_altitude,
				int2 chunk_pos,
				sample_transform<float2, op_f2> position_transform,
				PlaneHeightNoise<PerlinNoise_f2> terrain_altitude_noise,
				PlaneHeightNoise<PerlinNoise_f2> terrain_texture_noise,
				int3 chunk_size, out NativeArray<ushort> chunk)
			{
				chunk = new(chunk_size.volume(), Allocator.Persistent);
				this.grass_id = grass_id;
				this.snow_id = snow_id;
				this.soil_id = soil_id;
				this.stone_id = stone_id;
				this.air_id = air_id;
				this.position_transform = position_transform;
				this.grass_max_altitude = grass_max_altitude;
				this.snow_min_altitude = snow_min_altitude;
				this.chunk_pos = chunk_pos;
				this.terrain_altitude_noise = terrain_altitude_noise;
				this.terrain_texture_noise = terrain_texture_noise;
				coordinate = new(chunk_size);
				this.chunk = chunk;
			}
		}
	}
}