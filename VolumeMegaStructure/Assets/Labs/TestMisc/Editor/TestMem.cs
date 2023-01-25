using PrototypePackages.PrototypeUtils;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using static VolumeMegaStructure.Util.Collection.MemUtil;
namespace Labs.TestMisc.Editor
{
	public class TestMem : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestMem")]
		static void ShowWindow()
		{
			var window = GetWindow<TestMem>();
			window.titleContent = new("TestMem");
			window.Show();
		}
		unsafe void OnEnable()
		{
			var p_array = New<NativeArray<int>>(new(3, Allocator.TempJob), Allocator.TempJob);
			var array = *p_array;
			array[0] = 1;
			array[1] = 2;
			array[2] = 3;
			Debug.Log((*p_array).ToArray().JoinString(","));
			p_array->Dispose();
			Dispose(p_array, Allocator.TempJob);
		}

	}
}