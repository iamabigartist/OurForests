using UnityEditor;
using UnityEngine;
namespace Labs.Lab3_TestGPUBuffer.Editor
{
    public class TestInstantCutMeshIndex : EditorWindow
    {
        [MenuItem( "Labs/Labs.Lab3_TestGPUBuffer.Editor/TestInstantCutMeshIndex" )]
        private static void ShowWindow()
        {
            var window = GetWindow<TestInstantCutMeshIndex>();
            window.titleContent = new GUIContent( "TestInstantCutMeshIndex" );
            window.Show();
        }

        Mesh m_mesh;

        void Run1()
        {

        }

        void OnEnable()
        {
            m_mesh = new Mesh();

        }

        private void OnGUI()
        {

        }
    }
}
