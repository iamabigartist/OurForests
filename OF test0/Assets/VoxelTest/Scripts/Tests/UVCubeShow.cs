using System;
using System.Linq;
using UnityEngine;
namespace VoxelTest.Tests
{
    public class UVCubeShow : MonoBehaviour
    {
        MeshFilter mesh_filter;
        MeshRenderer mesh_renderer;

        void Start()
        {
            mesh_filter = gameObject.AddComponent<MeshFilter>();
            mesh_renderer = gameObject.AddComponent<MeshRenderer>();
            GenerateUVCube();
        }

        void GenerateUVCube()
        {
            var m_triangle = Enumerable.Range( 0, 6 * 6 ).ToArray();

        }

    }
}
