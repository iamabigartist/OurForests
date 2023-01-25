using UnityEditor;
namespace Labs.TestMisc.Editor
{
	public class TestSpan : EditorWindow
	{
		[MenuItem("Labs/Labs.Lab0_Misc.Editor/TestSpan")]
		static void ShowWindow()
		{
			var window = GetWindow<TestSpan>();
			window.titleContent = new("TestSpan");
			window.Show();
		}

		int a;

		ref int GetMyA() { return ref a; }

		int GetAA() { return a; }

		void OnEnable()
		{
			GetMyA() = 2;
		}

		void OnGUI() {}
	}
}