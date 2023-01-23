using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
namespace Labs.TestMisc.Editor
{
	/// <summary>
	///     结论：buffer以float结构为基础，也就是4的倍数是可以的。
	/// </summary>
	public class TestBufferSize : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestBufferSize")]
		static void ShowWindow()
		{
			var window = GetWindow<TestBufferSize>();
			window.titleContent = new("TestBufferSize");
			window.Show();
		}

		struct VoxelRenderMaterial
		{
			float3 base_color;
			float smoothness;
			float3 emission;
			float metallic;
			float ambient_occlusion;
		};

		void OnEnable()
		{
			var buffer = new ComputeBuffer(100, 9 * sizeof(float));
			Debug.Log(buffer.count);
			Debug.Log(buffer.stride);
			buffer.Release();
		}

		void OnGUI() {}
	}
}