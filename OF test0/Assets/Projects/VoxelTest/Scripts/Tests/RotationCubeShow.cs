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

            up_index_string = up_index.ToString();
            forward_index_string = forward_index.ToString();
            surface_index_string = surface_index.ToString();
        }


        string up_index_string, forward_index_string, surface_index_string;

        void OnGUI()
        {
            GUILayout.Label( nameof(up_index) );
            up_index_string = GUILayout.TextField( up_index_string );
            GUILayout.Label( nameof(forward_index) );
            forward_index_string = GUILayout.TextField( forward_index_string );
            GUILayout.Label( nameof(surface_index) );
            surface_index_string = GUILayout.TextField( surface_index_string );
            if (GUILayout.Button( "Reload" ))
            {
                if (!int.TryParse( up_index_string, out up_index )) { up_index_string = up_index.ToString(); }
                if (!int.TryParse( forward_index_string, out forward_index )) { forward_index_string = forward_index.ToString(); }
                if (!int.TryParse( surface_index_string, out surface_index )) { surface_index_string = surface_index.ToString(); }
                GenerateSurfaceOnce();
                RotateSourceCubeOnce();
            }
        }

        void GenerateSurfaceOnce()
        {
            var cur_mesh = new Mesh();
            VoxelUVGeneration.GetSurface(
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
            transform.GetChild( 0 ).rotation = VoxelUVGeneration.LookRotation( up_index, forward_index );
        }

    }
}
