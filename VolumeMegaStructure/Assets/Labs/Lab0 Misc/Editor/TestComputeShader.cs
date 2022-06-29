using PrototypeUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace Labs.Lab0_Misc.Editor
{
	public class TestComputeShader : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestCompuetShader")]
		static void ShowWindow()
		{
			var window = GetWindow<TestComputeShader>();
			window.titleContent = new("TestCompuetShader");
			window.Show();
		}

		const MeshUpdateFlags FAST_SET_FLAG =
			MeshUpdateFlags.DontValidateIndices |
			MeshUpdateFlags.DontNotifyMeshUsers |
			MeshUpdateFlags.DontRecalculateBounds;

		string index_buffer_array_string;

		void OnEnable()
		{
			var test_shader1 = Resources.Load<ComputeShader>("InitIndexBuffer");
			Mesh mesh = new();
			mesh.SetIndexBufferParams(100, IndexFormat.UInt32);
			mesh.indexBufferTarget |= GraphicsBuffer.Target.Structured;
			var index_buffer = mesh.GetIndexBuffer();
			Debug.Log(index_buffer.count);
			test_shader1.SetBuffer(0, "index_buffer", index_buffer);
			test_shader1.Dispatch(0, 100, 1, 1);
			var index_buffer_array = new int[100];
			index_buffer.GetData(index_buffer_array);
			index_buffer.Release();
			index_buffer_array_string = index_buffer_array.ListToString(".");
			Debug.Log(index_buffer_array_string);
		}

		void OnGUI() {}
	}
}