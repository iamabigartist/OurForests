using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using VolumeMegaStructure.Util.JobSystem;
namespace Labs.Lab0_Misc.Editor
{
	public class TestSizes : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestSizes")]
		static void ShowWindow()
		{
			var window = GetWindow<TestSizes>();
			window.titleContent = new("TestSizes");
			window.Show();
		}

		void OnEnable()
		{
			Debug.Log(UnsafeUtility.SizeOf(typeof(byte3)));
			Debug.Log(UnsafeUtility.SizeOf(typeof(byte2)));
		}

	}
}