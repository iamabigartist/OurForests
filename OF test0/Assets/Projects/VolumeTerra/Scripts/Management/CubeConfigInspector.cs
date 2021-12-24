using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
namespace VolumeTerra.Management
{
    [CustomEditor( typeof(CubeConfig) )]
    public class CubeConfigInspector : Editor
    {
        PreviewRenderUtility m_renderUtility;

        void InitRenderUtility()
        {
            m_renderUtility = new PreviewRenderUtility();
        }

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
            InitRenderUtility();
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
