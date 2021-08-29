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
    }
}
