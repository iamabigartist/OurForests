using System;
using Unity.Mathematics;
using UnityEngine;
namespace MUtility
{
    [Serializable]
    public struct Triangle
    {
        public Vector3 x;
        public Vector3 y;
        public Vector3 z;
        public Triangle(Vector3 x, Vector3 y, Vector3 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public static class VectorUtility
    {



    }
}
