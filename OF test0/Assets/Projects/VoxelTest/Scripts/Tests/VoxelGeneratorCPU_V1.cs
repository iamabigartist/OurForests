using System;
using RenderingTest.Components;
using UnityEditor;
using UnityEngine;
using VolumeTerra.DataDefinition;
using VolumeTerra.Generate;
namespace VoxelTest.Tests
{
    public class VoxelGeneratorCPU_V1 : MonoBehaviour
    {
        public Vector3Int TerrainSize = new Vector3Int( 100, 10, 100 );
        public float stone_height = 3;
        public float soil_height = 1;
        public Vector2 NoiseOffset;
        public float NoiseScale = 1;

        public GameObject m_prefab;
        Material prefab_material;
        Chunk Chunk;
        MeshFilter m_meshFilter;
        MeshRenderer m_meshRenderer;

        void Start()
        {
            Chunk.Cubes[0] = m_prefab.GetComponent<MeshFilter>().sharedMesh;
            prefab_material = m_prefab.GetComponent<MeshRenderer>().sharedMaterial;

            m_meshFilter = GetComponent<MeshFilter>();
            m_meshRenderer = GetComponent<MeshRenderer>();

            m_meshRenderer.material = prefab_material;
        }

        void OnGUI()
        {
            if (GUILayout.Button( "Regenerate" ))
            {
                GenerateOnce();
            }
        }

        void OnValidate()
        {
            if (Application.isPlaying)
            {
                GenerateOnce();
            }
        }

        void GenerateOnce()
        {
            var matrix = new VolumeMatrix<int>( TerrainSize );
            matrix.GenerateSimpleTerrain( stone_height, soil_height, NoiseOffset, NoiseScale );
            Debug.Log( $"{matrix.Position( 7128 )}" );
            Chunk = new Chunk( matrix );
            Chunk.VolumeMatrixToSegmentLists();
            Chunk.SegmentListToResultMesh();
            m_meshFilter.sharedMesh = Chunk.result_mesh;
        }

        void OnApplicationQuit()
        {
            var generated_path = "Assets/GeneratedResults/";
            AssetDatabase.CreateAsset( Chunk.result_mesh, generated_path + $"{name}ResultMesh" + ".mesh" );
        }

    }
}
