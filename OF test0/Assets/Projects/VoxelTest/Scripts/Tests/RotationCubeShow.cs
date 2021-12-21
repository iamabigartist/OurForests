using System.Linq;
using UnityEngine;
using VolumeTerra.DataDefinition;
namespace VoxelTest.Tests
{
    public class RotationCubeShow : MonoBehaviour
    {
        public int surface_index;
        public int up_index;
        public int forward_index;

        MeshFilter Filter;
        void Start()
        {
            up_index = 2;
            Filter = GetComponent<MeshFilter>();
        }

        void OnGUI()
        {
            GUILayout.Label( nameof(up_index) );
            int.TryParse( GUILayout.TextField( up_index.ToString() ), out up_index );
            GUILayout.Label( nameof(forward_index) );
            int.TryParse( GUILayout.TextField( forward_index.ToString() ), out forward_index );
            GUILayout.Label( nameof(surface_index) );
            int.TryParse( GUILayout.TextField( surface_index.ToString() ), out surface_index );
            if (GUILayout.Button( "Reload" ))
            {
                GenerateSurfaceOnce();
                RotateSourceCubeOnce();
            }
        }

        void GenerateSurfaceOnce()
        {
            var cur_mesh = new Mesh();
            SurfaceNormalDirection.GetSurface(
                surface_index,
                up_index,
                forward_index,
                out var surface_vertices,
                out var surface_uv );
            cur_mesh.SetVertices( surface_vertices );
            cur_mesh.SetUVs( 0, surface_uv );
            cur_mesh.SetTriangles( Enumerable.Range( 0, 6 ).ToArray(), 0 );
            cur_mesh.RecalculateBounds();
            cur_mesh.RecalculateNormals();
            cur_mesh.RecalculateTangents();
            Filter.sharedMesh = cur_mesh;
        }

        void RotateSourceCubeOnce()
        {
            transform.GetChild( 0 ).rotation = SurfaceNormalDirection.LookRotation( up_index, forward_index );
        }

    }
}
