using RenderingTest.Scripts.Configs;
using UnityEditor;
using UnityEngine;
namespace RenderingTest.Scripts
{
    [CustomEditor( typeof(CubeConfig) )]
    public class CubeConfigInspector : Editor
    {


        public static GameObject CreatePreviewModel(Mesh mesh, Material material)
        {
            var gameObject = new GameObject( "Preview Model" );
            var filter = gameObject.AddComponent<MeshFilter>();
            var renderer = gameObject.AddComponent<MeshRenderer>();
            filter.mesh = mesh;
            renderer.material = material;
            return gameObject;
        }

        CubeConfig _target;
        GameObject curModel;
        Editor curEditor;

        void RegenerateEditor()
        {
            if (_target.material != null && _target.mesh != null)
            {
                curModel = CreatePreviewModel( _target.mesh, _target.material );
                curEditor = CreateEditor( curModel );
            }
        }

        void OnEnable()
        {
            _target = (CubeConfig)target;
            RegenerateEditor();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (curEditor != null)
            {
                curEditor.OnInteractivePreviewGUI(
                    GUILayoutUtility.GetRect( 256, 256 ), GUIStyle.none );
                curEditor.OnPreviewSettings();
            }

            if (GUILayout.Button( "Regenerate" ))
            {
                RegenerateEditor();
            }
        }
    }
}
