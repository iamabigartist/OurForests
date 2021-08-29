﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumeTerra.DataDefinition;
using VolumeTerra.Generate;
namespace Tests
{
    public class ShaderData<T>
    {
        public int id;
        public T data;

        public ShaderData(int id, T data)
        {
            this.id = id;
            this.data = data;
        }
    }
    public class VoxelGenerator : MonoBehaviour
    {
        Mesh m_mesh;
        MeshFilter m_meshFilter;
        Dictionary<Vector3, int> QuadCenterToIndexDictionary;
        Vector3[] QuadVertices;
        Vector3[] QuadCenters;
        int[] m_triangles;


        public int VolumeSize = 100;
        public int Threshold = 5;
        public float GridSize = 1;

        VolumeMatrix<float> volume_matrix;

        ComputeShader cs;
        (
            ShaderData<Vector3Int> volume_number_size,
            ShaderData<Vector3Int> cube_number_size,
            ShaderData<float> threshold,
            ShaderData<ComputeBuffer> volume_matrix,
            ShaderData<float> grid_size,
            ShaderData<ComputeBuffer> quads,
            ShaderData<ComputeBuffer> quad_centers
            ) cs_data;


        void InitShader()
        {
            cs = Resources.Load<ComputeShader>( "VoxelGeneratorCS" );
            cs_data =
                (
                new ShaderData<Vector3Int>(
                    Shader.PropertyToID( nameof(cs_data.volume_number_size) ),
                    volume_matrix.volume_size ),
                new ShaderData<Vector3Int>(
                    Shader.PropertyToID( nameof(cs_data.cube_number_size) ),
                    volume_matrix.cube_size ),
                new ShaderData<float>(
                    Shader.PropertyToID( nameof(cs_data.threshold) ), Threshold ),
                new ShaderData<ComputeBuffer>(
                    Shader.PropertyToID( nameof(cs_data.volume_matrix) ),
                    new ComputeBuffer(
                        volume_matrix.Count, sizeof(float),
                        ComputeBufferType.Constant ) ),
                new ShaderData<float>(
                    Shader.PropertyToID( nameof(cs_data.grid_size) ), GridSize ),
                new ShaderData<ComputeBuffer>(
                    Shader.PropertyToID( nameof(cs_data.quads) ),
                    new ComputeBuffer(
                        volume_matrix.CubeCount * 12, 2 * 3 * sizeof(float) * 3,
                        ComputeBufferType.Constant ) ),
                new ShaderData<ComputeBuffer>(
                    Shader.PropertyToID( nameof(cs_data.quad_centers) ),
                    new ComputeBuffer(
                        volume_matrix.CubeCount * 12, sizeof(float) * 3,
                        ComputeBufferType.Constant ) )
                );

        }

        void BindShader()
        {
            cs.SetVector(
                cs_data.volume_number_size.id,
                cs_data.volume_number_size.data.ToVector4() );
            cs.SetVector(
                cs_data.cube_number_size.id,
                cs_data.cube_number_size.data.ToVector4() );
            cs.SetFloat(
                cs_data.threshold.id,
                cs_data.threshold.data );
            cs.SetBuffer( 0,
                cs_data.volume_matrix.id,
                cs_data.volume_matrix.data );
            cs.SetFloat(
                cs_data.grid_size.id,
                cs_data.grid_size.data );
            cs.SetBuffer( 0,
                cs_data.quads.id,
                cs_data.quads.data );
            cs.SetBuffer( 0,
                cs_data.quad_centers.id,
                cs_data.quad_centers.data );
        }

        void RefreshBind()
        {
            cs.SetFloat(
                cs_data.threshold.id,
                cs_data.threshold.data );
            cs.SetFloat(
                cs_data.grid_size.id,
                cs_data.grid_size.data );
        }

        void GenerateMesh()
        {
            cs.Dispatch( 0,
                Mathf.CeilToInt( volume_matrix.Count / 1024f ), 1, 1 );
        }

        void GetMeshAndDict()
        {
            cs_data.quads.data.GetData( QuadVertices );
            m_mesh.vertices = QuadVertices;
            m_mesh.triangles = m_triangles;
            m_meshFilter.sharedMesh = m_mesh;

            // cs_data.quad_centers.data.GetData( QuadCenters );
            // QuadCenterToIndexDictionary =
            //     QuadCenters.
            //         Select(
            //             (position, index) => (position, index) ).
            //         ToDictionary(
            //             pair => pair.position,
            //             pair => pair.index );
        }

        void GenerateVolumeMatrix()
        {
            volume_matrix.GenerateRandom( new Vector2( 0, 10 ) );
            cs_data.volume_matrix.data.SetData( volume_matrix.data );
        }

        void Start()
        {
            volume_matrix = new VolumeMatrix<float>(
                Vector3Int.one * VolumeSize );
            QuadVertices = new Vector3[volume_matrix.VoxelMeshVertexCount];
            QuadCenters = new Vector3[volume_matrix.VoxelMeshQuadCount];
            QuadCenterToIndexDictionary = new Dictionary<Vector3, int>();

            m_meshFilter = GetComponent<MeshFilter>();

            m_triangles = new int[volume_matrix.VoxelMeshVertexCount / 3];
            for (int i = 0; i < m_triangles.Count(); i++)
            {
                m_triangles[i] = i;
            }
            m_mesh = new Mesh();


            InitShader();
            BindShader();
            GenerateVolumeMatrix();
            GenerateMesh();
            GetMeshAndDict();
            Debug.Log("1");
        }

        void OnGUI()
        {
            if (GUILayout.Button( "Regenerate volume matrix" ))
            {
                GenerateVolumeMatrix();
            }
        }

        void Update()
        {
            RefreshBind();
            GenerateMesh();
            GetMeshAndDict();
        }

        void OnDestroy()
        {
            cs_data.volume_matrix.data.Release();
            cs_data.quads.data.Release();
            cs_data.quad_centers.data.Release();
        }

    }
}