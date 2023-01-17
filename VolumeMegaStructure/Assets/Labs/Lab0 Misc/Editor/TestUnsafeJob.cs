using UnityEditor;
namespace Labs.Lab0_Misc.Editor
{
	// public unsafe struct NativePtrJob : IJob
	// {
	// 	NativeList<int> list;
	// 	NativeArray<int>* array;
	// 	public void Execute() {}
	// 	public NativePtrJob(out NativeArray<int>* array)
	// 	{
	// 		this.array=UnsafeUtility.Malloc()
	// 	}
	// }
	public class TestUnsafeJob : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestUnsafeJob")]
		static void ShowWindow()
		{
			var window = GetWindow<TestUnsafeJob>();
			window.titleContent = new("TestUnsafeJob");
			window.Show();
		}

		void OnEnable() {}

		void OnGUI() {}
	}
}