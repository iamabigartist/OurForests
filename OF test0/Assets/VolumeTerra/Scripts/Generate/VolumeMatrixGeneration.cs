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

        public static void GenerateSimpleNoise<T>(this VolumeMatrix<T> matrix, string seed)
        {
            var noisier = new SimplexNoiseGenerator( seed );
            for (int z = 0; z < matrix.volume_size.z; z++)
            {
                for (int y = 0; y < matrix.volume_size.y; y++)
                {
                    for (int x = 0; x < matrix.volume_size.x; x++)
                    {
                        matrix[x, y, z] = (dynamic)(noisier.coherentNoise( x, y, z ) + 1) * 5;
                    }
                }
            }
        }
    }
}
