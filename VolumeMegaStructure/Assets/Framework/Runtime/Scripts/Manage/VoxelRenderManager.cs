using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
namespace VolumeMegaStructure.Manage
{
	public struct VoxelColorTexture
	{
		float3 base_color;
		float smoothness;
		float metallic;
	}
	public class VoxelRenderManager : IDisposable
	{
		public List<VoxelColorTexture> textures;
		public Material material;
		public ComputeShader quad_gen_unit_to_vertex_buffer;
		public ComputeShader gen_quad6_index_buffer;
		// public ComputeShader vertex_buffer_editor;
		ComputeBuffer source_vertex_buffer; //暂时没用
		ComputeBuffer uv_buffer;
		ComputeBuffer normal_buffer;
		ComputeBuffer tangent_buffer;
		ComputeBuffer color_texture_buffer;

		public VoxelRenderManager(VoxelSourceTables sourceTables)
		{
			textures = new();
			material = new(Shader.Find("Shader Graphs/VoxelMeshLit_PureColor"));
			quad_gen_unit_to_vertex_buffer = Resources.Load<ComputeShader>("QuadGenUnitToVertexBuffer");
			gen_quad6_index_buffer = Resources.Load<ComputeShader>("GenQuad6IndexBuffer");
			InitMaterial(sourceTables);
		}

		void InitMaterial(VoxelSourceTables sourceTables)
		{
			color_texture_buffer = new(textures.Count, sizeof(float) * 5, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
			color_texture_buffer.SetData(textures);
			sourceTables.GetVoxelSourceRenderBuffers(out source_vertex_buffer, out uv_buffer, out normal_buffer, out tangent_buffer);
			material.SetBuffer("vertex_uvs", uv_buffer);
			material.SetBuffer("face_normals", normal_buffer);
			material.SetBuffer("face_tangents", tangent_buffer);
			material.SetBuffer("quad_color_textures", color_texture_buffer);
		}

		public void Dispose()
		{
			source_vertex_buffer.Release();
			uv_buffer.Release();
			normal_buffer.Release();
			tangent_buffer.Release();
			color_texture_buffer.Release();
		}
	}
}