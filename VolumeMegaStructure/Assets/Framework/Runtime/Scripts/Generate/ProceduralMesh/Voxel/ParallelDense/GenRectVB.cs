using Unity.Mathematics;
using UnityEngine;
using VolumeMegaStructure.Util;
using static Unity.Mathematics.math;
namespace VolumeMegaStructure.Generate.ProceduralMesh.Voxel.ParallelDense
{
	public static class GenRectVB
	{
		const int NUM_THREAD_X = 512;
		static readonly string[] KERNEL_NAMES =
		{
			"Main_PlusX",
			"Main_MnusX",
			"Main_PlusY",
			"Main_MnusY",
			"Main_PlusZ",
			"Main_MnusZ"
		};

		const string CS_PATH = "RectUnitToVB";
		const string MATRIX_SIZE = "matrix_size";
		const string DIR = "dir";
		const string INDEX_RECT_VB_START = "i_rect_vb_start";
		const string RECT_BUFFER_LEN = "rect_buffer_len";
		const string RECT_BUFFER = "rect_buffer";
		const string VERTEX_BUFFER = "vertex_buffer";

		static int[] kernel_indices;
		static int id_matrix_size;
		static int id_dir;
		static int id_i_rect_vb_start;
		static int id_rect_buffer_len;
		static int id_rect_buffer;
		static int id_vertex_buffer;

		static ComputeShader cs;

		static GenRectVB()
		{
			id_matrix_size = Shader.PropertyToID(MATRIX_SIZE);
			id_vertex_buffer = Shader.PropertyToID(VERTEX_BUFFER);
			id_dir = Shader.PropertyToID(DIR);
			id_i_rect_vb_start = Shader.PropertyToID(INDEX_RECT_VB_START);
			id_rect_buffer_len = Shader.PropertyToID(RECT_BUFFER_LEN);
			id_rect_buffer = Shader.PropertyToID(RECT_BUFFER);

			cs = Resources.Load<ComputeShader>(CS_PATH);
			kernel_indices = new int[6];
			for (int i = 0; i < 6; i++) { kernel_indices[i] = cs.FindKernel(KERNEL_NAMES[i]); }
		}

		static void PlanDir(int dir, int i_rect_vb_start, int rect_buffer_len, ComputeBuffer rect_buffer, GraphicsBuffer vertex_buffer)
		{
			if (rect_buffer_len == 0) { return; }
			int cur_kernel_index = kernel_indices[dir];
			cs.SetInt(id_dir, dir);
			cs.SetInt(id_i_rect_vb_start, i_rect_vb_start);
			cs.SetInt(id_rect_buffer_len, rect_buffer_len);
			cs.SetBuffer(cur_kernel_index, id_rect_buffer, rect_buffer);
			cs.SetBuffer(cur_kernel_index, id_vertex_buffer, vertex_buffer);
			int thread_group_x = (int)ceil(rect_buffer_len / (float)NUM_THREAD_X);
			cs.Dispatch(cur_kernel_index, thread_group_x, 1, 1);
		}

		public static void Plan6Dir(int3 matrix_size, GraphicsBuffer vertex_buffer, int2[] rect_array_start_lens, ComputeBuffer[] rect_buffers)
		{
			cs.SetInts(id_matrix_size, matrix_size.x, matrix_size.y, matrix_size.z);
			for (int i = 0; i < 6; i++)
			{
				var (i_rect_vb_start, rect_buffer_len) = rect_array_start_lens[i];
				PlanDir(i, i_rect_vb_start, rect_buffer_len, rect_buffers[i], vertex_buffer);
			}
		}
	}
}