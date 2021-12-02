using System.Collections.Generic;
using System.Linq;
using MUtility;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeTerra.DataDefinition;
using VolumeTerra.Generate;
public class VolumeGenerateTest : MonoBehaviour
{

    public int Threshold = 5;
    public int VolumeSize = 50;
    VolumeMatrix<float> volume_matrix;

    public GameObject m_prefab;
    Mesh prefab_mesh;
    Material prefab_material;

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


    }


    void GenerateVolumeMatrix()
    {
        volume_matrix.GenerateCoherentNoise( new Vector2( 0, 10 ), "aszdas" );
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

        var mesh = new Mesh()
        {
            indexFormat = IndexFormat.UInt32
        };
        mesh.vertices = ll_vertices.ToArray();
        mesh.triangles = ll_triangles.ToArray();
        mesh.uv = ll_uv1.ToArray();
        mesh.uv2 = ll_uv2.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        m_meshFilter.sharedMesh = mesh;
    }

}