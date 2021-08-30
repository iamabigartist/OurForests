using UnityEngine;
using VolumeTerra.DataDefinition;
namespace VolumeTerra.Generate
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

        public static void GenerateCoherentNoise<T>(this VolumeMatrix<T> matrix, Vector2 range, string seed)
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

        public static void GenerateSphere<T>(this VolumeMatrix<T> matrix, T inside_value, T outside_value, float radius, Vector3Int mid_point)
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
    }
}
