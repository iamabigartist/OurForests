using UnityEngine;
using VolumeTerra.Scripts.DataDefinition;
namespace VolumeTerra.Scripts.Generate
{
    public static class VolumeMatrixGeneration
    {
        /// <param name="range">The range of the volume value</param>
        public static void GenerateRandom<T>(this VolumeMatrix<T> matrix, Vector2 range)
        {
            for (int i = 0; i < matrix.Count; i++)
            {
                matrix[i] = (dynamic)Random.Range( range.x, range.y );
            }
        }

        public static void GenerateCoherentNoiseThreshold<T>(this VolumeMatrix<T> matrix, Vector2 range, string seed)
        {
            var noisier = new SimplexNoiseGenerator( seed );
            for (int z = 0; z < matrix.volume_size.z; z++)
            {
                for (int y = 0; y < matrix.volume_size.y; y++)
                {
                    for (int x = 0; x < matrix.volume_size.x; x++)
                    {
                        matrix[x, y, z] = (dynamic)
                            ((noisier.coherentNoise( x, y, z ) + 1) / 2 *
                            (range.y - range.x) + range.x);
                        // matrix[x, y, z] = (dynamic)(noisier.coherentNoise( x, y, z ) + 1) * 5;
                    }
                }
            }
        }

        public static void GenerateSphereThreshold<T>(this VolumeMatrix<T> matrix, T inside_value, T outside_value, float radius, Vector3Int mid_point)
        {
            for (int z = 0; z < matrix.volume_size.z; z++)
            {
                for (int y = 0; y < matrix.volume_size.y; y++)
                {
                    for (int x = 0; x < matrix.volume_size.x; x++)
                    {
                        float distance = Vector3.Distance(
                            new Vector3( x, y, z ),
                            mid_point );
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
            this VolumeMatrix<int> matrix,
            float stone_height_scale,
            float soil_height_scale,
            Vector2 noise_offset,
            float noise_scale = 1)
        {

            Vector2 transform(Vector2 position)
            {
                return position * noise_scale + noise_offset;
            }


            for (int y = 0; y < matrix.volume_size.y; y++)
            {
                for (int x = 0; x < matrix.volume_size.x; x++)
                {
                    var stone_noise_position = transform( new Vector2( x, y ) );
                    float stone_height = Mathf.PerlinNoise( stone_noise_position.x, stone_noise_position.y );
                    var soil_noise_position = transform( new Vector2( x + 5000, y + 5000 ) );
                    float soil_height = Mathf.PerlinNoise( soil_noise_position.x, soil_noise_position.y );
                    for (int z = 0; z < matrix.volume_size.z; z++)
                    {
                        if (z <= stone_height)
                        {
                            matrix[x, y, z] = 2;
                        }
                        else if (z <= stone_height + soil_height)
                        {
                            matrix[x, y, z] = 1;
                        }
                        else
                        {
                            matrix[x, y, z] = 0;
                        }
                    }

                }
            }

        }
    }
}
