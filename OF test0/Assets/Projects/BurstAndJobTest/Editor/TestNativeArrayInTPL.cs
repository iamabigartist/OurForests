using System.Threading.Tasks;
using PrototypeUtils;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
namespace BurstAndJobTest
{
	public class TestNativeArrayInTPL : EditorWindow
	{
		[MenuItem("BurstAndJobTest/TestNativeArrayInTPL")]
		static void ShowWindow()
		{
			var window = GetWindow<TestNativeArrayInTPL>();
			window.titleContent = new("TestNativeArrayInTPL");
			window.Show();
		}

		NativeArray<int> array1;

		void OnEnable()
		{
			array1 = new(1000, Allocator.Persistent);
			Parallel.For(0, 1000, i =>
			{
				array1[i] = i;
			});
			Debug.Log(array1.ToArray()[..100].ToMString(","));
		}

		void OnGUI() {}
	}
}