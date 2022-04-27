using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI.UssLab.Editor
{
	public class TestIntegerField : EditorWindow
	{
		[MenuItem("Labs/UI.UssLab.Editor/TestIntegerField")]
		static void ShowWindow()
		{
			var window = GetWindow<TestIntegerField>();
			window.titleContent = new("TestIntegerField");
			window.Show();
		}

		void CreateGUI()
		{
			var integer_field = new IntegerField();
			rootVisualElement.Add(integer_field );
			integer_field.AddToClassList("my-integer-field");
			var uss = Resources.Load<StyleSheet>("MyIntegerField");
			rootVisualElement.styleSheets.Add(uss);
			integer_field.value = 1023;
		}
	}
}