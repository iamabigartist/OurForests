using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using VolumeTerra.DataDefinition;
using VolumeTerra.Generate;
namespace VoxelTest.Tests
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
    public class VoxelGeneratorV1 : MonoBehaviour
    {
        Mesh m_mesh;
        MeshFilter m_meshFilter;
        MeshCollider m_meshCollider;
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


        void Start()
        {
            volume_matrix = new VolumeMatrix<float>(
                Vector3Int.one * VolumeSize );
            QuadVertices = new Vector3[volume_matrix.VoxelMeshVertexCount];
            QuadCenters = new Vector3[volume_matrix.VoxelMeshQuadCount];
            QuadCenterToIndexDictionary = new Dictionary<Vector3, int>();

            m_meshFilter = GetComponent<MeshFilter>();
            m_meshCollider = GetComponent<MeshCollider>();

            m_triangles = new int[volume_matrix.VoxelMeshVertexCount / 3];
            for (int i = 0; i < m_triangles.Count(); i++)
            {
                m_triangles[i] = i;
            }
            m_mesh = new Mesh();
            m_mesh.indexFormat = IndexFormat.UInt32;



            InitShader();
            BindShader();

            GenerateVolumeMatrix();
            GenerateMesh();
            GetMeshAndDict();
        }

        // bool contains_1;
        // bool contains_0;
        //
        //

        void Update()
        {
            // GenerateVolumeMatrix();
            // RefreshBind();
            // GenerateMesh();
            // GetMeshAndDict();
            // contains_0 = QuadCenters.Contains( Vector3.zero );
            // contains_1 = QuadCenters.Contains( Vector3.one );
        }

        // void OnValidate()
        // {
        //     GenerateVolumeMatrix();
        //     RefreshBind();
        //     GenerateMesh();
        //     GetMeshAndDict();
        // }

        void InitShader()
        {
            cs = Resources.Load<ComputeShader>( "VoxelGeneratorCSV1" );
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
                        volume_matrix.Count, sizeof(float) ) ),
                new ShaderData<float>(
                    Shader.PropertyToID( nameof(cs_data.grid_size) ), GridSize ),
                new ShaderData<ComputeBuffer>(
                    Shader.PropertyToID( nameof(cs_data.quads) ),
                    new ComputeBuffer(
                        volume_matrix.VoxelMeshQuadCount, 2 * 3 * sizeof(float) * 3,
                        ComputeBufferType.Counter ) ),
                new ShaderData<ComputeBuffer>(
                    Shader.PropertyToID( nameof(cs_data.quad_centers) ),
                    new ComputeBuffer(
                        volume_matrix.VoxelMeshQuadCount, sizeof(float) * 3 ) )
                );

        }

        void BindShader()
        {
            var v3i1 = cs_data.volume_number_size.data;
            cs.SetInts(
                cs_data.volume_number_size.id,
                v3i1.x, v3i1.y, v3i1.z );

            var v3i2 = cs_data.cube_number_size.data;
            cs.SetInts(
                cs_data.cube_number_size.id,
                v3i2.x, v3i2.y, v3i2.z );

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
            cs_data.threshold.data = Threshold;
            cs_data.grid_size.data = GridSize;
            cs.SetFloat(
                cs_data.threshold.id,
                cs_data.threshold.data );
            cs.SetFloat(
                cs_data.grid_size.id,
                cs_data.grid_size.data );
        }

        void GenerateMesh()
        {
            cs_data.quads.data.SetCounterValue( 0 );
            cs.Dispatch( 0,
                Mathf.CeilToInt( volume_matrix.CubeCount / 1024f ), 1, 1 );
        }

        void GetMeshAndDict()
        {
            cs_data.quads.data.GetData( QuadVertices );
            m_mesh.vertices = QuadVertices;
            m_mesh.triangles = m_triangles;
            m_mesh.RecalculateBounds();
            m_mesh.RecalculateNormals();
            m_meshFilter.sharedMesh = m_mesh;
            m_meshCollider.sharedMesh = m_mesh;

            cs_data.quad_centers.data.GetData( QuadCenters );
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
            volume_matrix.GenerateSphere( 10, 0, 7, volume_matrix.CenterPoint );
            // volume_matrix.GenerateCoherentNoise( new Vector2( 0, 10 ), "new Vector2( 0, 10 )" );
            cs_data.volume_matrix.data.SetData( volume_matrix.data );
        }


        void OnGUI()
        {
            // if (GUILayout.Button( "Regenerate volume matrix" ))
            // {
            //     GenerateVolumeMatrix();
            //     RefreshBind();
            //     GenerateMesh();
            //     GetMeshAndDict();
            // }
        }


        void OnDestroy()
        {
            cs_data.volume_matrix.data.Release();
            cs_data.quads.data.Release();
            cs_data.quad_centers.data.Release();
        }
    }

}
