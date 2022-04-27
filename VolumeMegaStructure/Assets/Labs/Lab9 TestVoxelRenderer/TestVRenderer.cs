using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static VolumeMegaStructure.Generate.ProceduralMesh.Voxel.GenVoxelSourceTables;
using static VolumeMegaStructure.Util.VoxelProcessUtility;
using Random = Unity.Mathematics.Random;
namespace Labs.Lab9_TestVoxelRenderer
{
	public class TestVRenderer : MonoBehaviour
	{

	#region Config

	#endregion

	#region Data

		Texture2DArray texture_2d_array;

	#endregion

	#region Reference

		GameObject v4_voxel_mesh;
		GameObject decompose_voxel_mesh;

	#endregion

	#region Process

	#region Init

		void Init2GameObject()
		{
			v4_voxel_mesh = new(nameof(v4_voxel_mesh),
				typeof(MeshFilter), typeof(MeshRenderer));
			decompose_voxel_mesh = new(nameof(decompose_voxel_mesh),
				typeof(MeshFilter), typeof(MeshRenderer));
			v4_voxel_mesh.transform.parent = transform;
			decompose_voxel_mesh.transform.parent = transform;
		}

		void InitTexture2DArray()
		{
			texture_2d_array = Resources.LoadAll<Texture2D>("Lab9Textures").select(t => t.CPU_DXT1ToDXT5()).GenTexture2DArray();
		}

		void Init2Material()
		{
			v4_voxel_mesh.GetComponent<MeshRenderer>().material = new(Shader.Find("Shader Graphs/VoxelMeshLit_V4"));
			v4_voxel_mesh.GetComponent<MeshRenderer>().material.SetTexture("TArray", texture_2d_array);
			decompose_voxel_mesh.GetComponent<MeshRenderer>().material = new(Shader.Find("Shader Graphs/VoxelMeshLit_Decompose"));
			decompose_voxel_mesh.GetComponent<MeshRenderer>().material.SetTexture("TArray", texture_2d_array);
		}

	#endregion

	#region GenMesh

		void GenRandomQuad(int seed, out float3[] vertices, out int[] face_uv_texture_indices)
		{
			var random = Random.CreateFromIndex((uint)seed);
			int i_up = random.NextInt(0, 6);
			int i_forward;
			do { i_forward = random.NextInt(0, 6); } while (!ValidLookRotation(i_up, i_forward));
			int i_face = random.NextInt(0, 6);
			int i_texture = random.NextInt(0, texture_2d_array.depth);
			vertices = new float3[4];
			face_uv_texture_indices = new int[4];
			i_rotation_i_face_i_vertex_Compose(i_up, i_forward, i_face, 0, out var i0);
			vertices = FixedUVVertexTable[i0..(i0 + 4)];
			for (int i_vertex = 0; i_vertex < 4; i_vertex++)
			{
				i_texture_i_face_i_vertex_Compose(i_texture, i_face, i_vertex, out var composed_index);
				face_uv_texture_indices[i_vertex] = composed_index;
			}
		}

		//1. 如果只转化buffer的话，需要一个已经提前gen 好的mesh加上vertex buffer的定义,因为set vertex buffer params 函数只在mesh里面有。
		//2. 因此还是需要两个各自生成单独的mesh，在job 里面对于GenRandomQuad 函数的结果进行处理。
		//3. 写两个job 吧。
		void GetV4Buffer(in float3[] vertices, in int[] face_uv_texture_indices)
		{
			var a = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, 100, 4);
			var a = new Mesh();
			a.SetVertexBufferParams();
		}
		
		void GetComposeBuffer()
		{
			
		}

	#endregion

	#endregion

	#region Interface

		[ContextMenu("Regenerate")]
		void Regenerate()
		{

		}

	#endregion

	#region Unity Entry

		void Start()
		{
			Init2GameObject();
			InitTexture2DArray();
			Init2Material();
		}

	#endregion
	}
}