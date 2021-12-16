using System;
using System.Collections.Generic;
using System.Linq;
using MUtility;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeTerra.DataDefinition;
using VolumeTerra.Generate;
namespace RenderingTest.Tests
{
    public class VolumeGenerateTest : MonoBehaviour
    {

        public int Threshold = 5;
        public int VolumeSize = 50;
        VolumeMatrix<float> volume_matrix;

        public GameObject m_prefab;
        Mesh prefab_mesh;
        Material prefab_material;

        Mesh m_mesh;
        LinkedList<Vector3> ll_vertices;
        LinkedList<int> ll_triangles;
        LinkedList<Vector2> ll_uv1;
        LinkedList<Vector2> ll_uv2;
        MeshFilter m_meshFilter;
        MeshRenderer m_meshRenderer;



        void Start()
        {
            volume_matrix = new VolumeMatrix<float>( Vector3Int.one * VolumeSize );

            prefab_mesh = m_prefab.GetComponent<MeshFilter>().sharedMesh;
            prefab_material = m_prefab.GetComponent<MeshRenderer>().sharedMaterial;

            ll_triangles = new LinkedList<int>();
            ll_vertices = new LinkedList<Vector3>();
            ll_uv1 = new LinkedList<Vector2>();
            ll_uv2 = new LinkedList<Vector2>();

            m_meshFilter = GetComponent<MeshFilter>();
            m_meshRenderer = GetComponent<MeshRenderer>();

            m_meshRenderer.material = prefab_material;

            GenerateVolumeMatrix();
            AddAllMesh();


            Debug.Log( $"Memory in GB: {m_mesh.GetObjectByteSize() / (1024f * 1024f * 1024f)}" );
            Debug.Log( $"Vertices Length: {ll_vertices.Count}" );

        }


        void GenerateVolumeMatrix()
        {
            volume_matrix.GenerateCoherentNoiseThreshold( new Vector2( 0, 10 ), "aszdas" );
        }

        void AddOnce(Vector3 position)
        {

            var world_vertices = prefab_mesh.vertices;
            for (int i = 0; i < world_vertices.Length; i++)
            {
                world_vertices[i] += position;
            }
            var offseted_triangles = prefab_mesh.triangles;
            for (int i = 0; i < offseted_triangles.Length; i++)
            {
                offseted_triangles[i] += ll_vertices.Count;
            }

            ll_vertices.AppendLast( world_vertices );
            ll_triangles.AppendLast( offseted_triangles );
            ll_uv1.AppendLast( prefab_mesh.uv );

        }

        void DeletePiece(
            LinkedListNode<Vector3> v_start_last,
            LinkedListNode<Vector2> uv1_start_last,
            LinkedListNode<Vector2> uv2_start_last,
            int v_count,
            LinkedListNode<int> t_start_last,
            int t_count)
        {
            for (int i = 0; i < v_count; i++)
            {
                ll_vertices.Remove( v_start_last.Next! );
                ll_uv1.Remove( uv1_start_last.Next! );
                ll_uv2.Remove( uv2_start_last.Next! );
            }

            for (int i = 0; i < t_count; i++)
            {
                ll_triangles.Remove( t_start_last.Next! );
            }

            for (var cur_t_node = t_start_last.Next; cur_t_node != null; cur_t_node = cur_t_node.Next)
            {
                cur_t_node.Value -= v_count;
            }

        }

        void AddAllMesh()
        {
            for (int z = 0; z < volume_matrix.volume_size.z; z++)
            {
                for (int y = 0; y < volume_matrix.volume_size.y; y++)
                {
                    for (int x = 0; x < volume_matrix.volume_size.x; x++)
                    {
                        if (volume_matrix[x, y, z] > Threshold) { AddOnce( new Vector3( x, y, z ) ); }
                    }
                }
            }

            m_mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                vertices = ll_vertices.ToArray(),
                triangles = ll_triangles.ToArray(),
                uv = ll_uv1.ToArray(),
                uv2 = ll_uv2.ToArray()
            };
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
            m_mesh.RecalculateTangents();
            m_meshFilter.mesh = m_mesh;
        }

        void OnDisable()
        {
            var f_mesh = m_meshFilter.mesh;
            Destroy( f_mesh );
            Destroy( m_meshFilter );
            Destroy( this );
            Debug.Log( $"{GC.GetTotalMemory( false )}" );
            GC.Collect();
            Debug.Log( $"{GC.GetTotalMemory( false )}" );


        }



    }

}
