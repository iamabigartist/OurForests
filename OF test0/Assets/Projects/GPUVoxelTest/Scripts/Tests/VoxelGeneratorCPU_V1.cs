﻿using System.Diagnostics;
using PrototypeUtils;
using UnityEngine;
using VolumeTerra.DataDefinition;
using VolumeTerra.Generate.Terrain;
using Debug = UnityEngine.Debug;
namespace GPUVoxelTest.Tests
{
    public class VoxelGeneratorCPU_V1 : MonoBehaviour
    {
        public Vector3Int TerrainSize = new(100, 10, 100);
        public float stone_height = 3;
        public float soil_height = 1;
        public Vector2 NoiseOffset;
        public float NoiseScale = 1;

        public GameObject m_prefab;
        Material prefab_material;
        ChunkMesh m_chunkMesh;
        MeshFilter m_meshFilter;
        MeshRenderer m_meshRenderer;

        void Start()
        {
            ChunkMesh.Cubes[0] = m_prefab.GetComponent<MeshFilter>().sharedMesh;
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


        void GenerateOnce()
        {
            var stop = new Stopwatch();

            stop.Start();
            var matrix = new VolumeMatrix<int>( TerrainSize );
            matrix.GenerateSimpleTerrain( stone_height, soil_height, NoiseOffset, NoiseScale );
            // Debug.Log( $"{matrix.Position( 7128 )}" );
            m_chunkMesh = new(matrix);
            stop.Stop();
            Debug.Log( $"GenerateSimpleTerrain: {stop.Get_ms()} ms" );

            stop.Restart();
            m_chunkMesh.General_VolumeMatrixToSegmentLists();
            stop.Stop();
            Debug.Log( $"General_VolumeMatrixToSegmentLists: {stop.Get_ms()} ms" );

            stop.Restart();
            m_chunkMesh.GenerateResultMesh();
            stop.Stop();
            Debug.Log( $"GenerateResultMesh: {stop.Get_ms()} ms" );

            //Mesh Optimise 系列通过重新排列VB或者IB数据来增加cache命中率，对于生成的美术模型来说会有用；但是如果你的生成模型使用固定的IB读取方式，那么这种方式不会增加cache命中率，并且还会打乱顺序。
            // stop.Restart();
            // m_chunkMesh.result_mesh.OptimizeReorderVertexBuffer();
            // stop.Stop();
            // Debug.Log( $"OptimizeReorderVertexBuffer: {stop.Get_ms()} ms" );

            stop.Restart();
            m_meshFilter.sharedMesh = m_chunkMesh.result_mesh;
            stop.Stop();
            Debug.Log( $"sharedMesh Assign: {stop.Get_ms()} ms" );
        }

        // void OnApplicationQuit()
        // {
        //     var generated_path = "Assets/GeneratedResults/";
        //     AssetDatabase.CreateAsset( ChunkMesh.result_mesh, generated_path + $"{name}ResultMesh" + ".mesh" );
        // }

    }
}
