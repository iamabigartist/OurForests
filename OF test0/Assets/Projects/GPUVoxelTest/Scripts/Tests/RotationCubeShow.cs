using System.Collections.Generic;
using MUtility;
using MyForestSystem;
using UnityEngine;
using VolumeTerra.Generate.SourceGenerator;
namespace GPUVoxelTest.Tests
{
    public class RotationCubeShow : MonoBehaviour
    {
        public int surface_index;
        public int up_index;
        public int forward_index;

        MeshFilter Filter;

        VoxelRotationInfoTable m_voxelRotationInfoTable;
        VoxelSourceMesh m_voxelSourceMesh;
        ComputeBuffer uv_buffer;
        ComputeBuffer normal_buffer;
        ComputeBuffer tangent_buffer;

        void Start()
        {
            up_index = 2;
            Filter = GetComponent<MeshFilter>();
            m_voxelSourceMesh = MainManager.VoxelSourceMesh;
            m_voxelRotationInfoTable = MainManager.VoxelRotationInfoTable;
            up_index_string = up_index.ToString();
            forward_index_string = forward_index.ToString();
            surface_index_string = surface_index.ToString();
            uv_buffer = new ComputeBuffer( 4, sizeof(float) * 2 );
            normal_buffer = new ComputeBuffer( 6, sizeof(float) * 3 );
            tangent_buffer = new ComputeBuffer( 6, sizeof(float) * 4 );
            m_voxelSourceMesh.SetMeshInfoBuffers( uv_buffer, normal_buffer, tangent_buffer );
            var material = GetComponent<MeshRenderer>().material;
            material.SetBuffer( "vertex_uvs", uv_buffer );
            material.SetBuffer( "face_normals", uv_buffer );
            material.SetBuffer( "face_tangents", uv_buffer );
            transform.GetChild( 0 ).GetComponent<MeshRenderer>().material = material;
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
                GenerateCubeOnce();
                RotateSourceCubeOnce();
            }
        }

        void GenerateSurfaceOnce()
        {
            var cur_mesh = new Mesh();
            var surface_vertex_uv_indices = new int[6];
            m_voxelRotationInfoTable.GetFaceVertexUVIndices( surface_index, up_index, forward_index, surface_vertex_uv_indices );
            var surface_uv_input = new Vector3[6];
            for (int i = 0; i < 6; i++)
            {
                surface_uv_input[i] = new Vector3( surface_vertex_uv_indices[i], surface_index, surface_index );
            }
            var surface_vertices = new Vector3[6];
            m_voxelSourceMesh.GetFaceVertices( surface_index, surface_vertices );
            cur_mesh.SetVertices( surface_vertices );
            cur_mesh.SetUVs( 0, surface_uv_input );
            cur_mesh.SetTriangles( (..6).ToArray(), 0 );
            cur_mesh.RecalculateBounds();
            cur_mesh.RecalculateNormals();
            cur_mesh.RecalculateTangents();
            Filter.sharedMesh = cur_mesh;
        }

        void GenerateCubeOnce()
        {
            var cur_mesh = new Mesh();
            var surface_vertex_uv_indices = new int[36];
            m_voxelRotationInfoTable.GetCubeVertexUVIndices( up_index, forward_index, surface_vertex_uv_indices );
            var surface_uv_input = new Vector3[36];
            for (int face_i = 0; face_i < 6; face_i++)
            {
                for (int local_vertex_i = 0; local_vertex_i < 6; local_vertex_i++)
                {
                    var vertex_i = face_i * 6 + local_vertex_i;
                    surface_uv_input[vertex_i] = new Vector3( surface_vertex_uv_indices[vertex_i], face_i, face_i );
                }
            }
            var cube_vertices = m_voxelSourceMesh.Vertices;
            cur_mesh.SetVertices( cube_vertices );
            cur_mesh.SetUVs( 0, surface_uv_input );
            cur_mesh.SetTriangles( (..36).ToArray(), 0 );
            cur_mesh.RecalculateBounds();
            cur_mesh.RecalculateNormals();
            cur_mesh.RecalculateTangents();
            Filter.sharedMesh = cur_mesh;
            var list = new List<Vector4>();
            cur_mesh.GetUVs( 0, list );
        }

        void RotateSourceCubeOnce()
        {
            transform.GetChild( 0 ).rotation = VoxelGenerationUtility.LookRotation( up_index, forward_index );
        }

        void OnDestroy()
        {
            uv_buffer.Release();
            normal_buffer.Release();
            tangent_buffer.Release();
        }

    }
}
