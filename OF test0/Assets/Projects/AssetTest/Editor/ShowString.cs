using UnityEditor;
using UnityEngine;
namespace AssetTest
{
    public class ShowString : EditorWindow
    {
        [MenuItem( "AssetTest.Editor/ShowString" )]
        private static void ShowWindow()
        {
            var window = GetWindow<ShowString>();
            window.titleContent = new GUIContent( "ShowString" );
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"{"人人为我我为人人".GetHashCode()}");
        }
    }
}
