using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel.Sequential256.Octree;
namespace Labs.TestMisc.Editor
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
			Debug.Log(UnsafeUtility.SizeOf<int3>());
			Debug.Log(UnsafeUtility.SizeOf<int>());
			Debug.Log(UnsafeUtility.SizeOf<OctRegion>());
		}

	}
}