using UnityEditor;
using UnityEngine;
using static PrototypeUtils.UIUtils;
namespace UI.ShowAndExecuteFunctionLab
{
    public class SceneGUI : EditorWindow
    {
        [MenuItem( "Labs/UI.ShowAndExecuteFunctionLab/SceneGUI" )]
        static void ShowWindow()
        {
            var window = GetWindow<SceneGUI>();
            window.titleContent = new GUIContent( "SceneGUI" );
            window.Show();
        }


        Camera m_camera;
        Transform target;

        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        Vector2 offset;

        void OnGUI()
        {
            target = (Transform)EditorGUILayout.ObjectField( target, typeof(Transform), true );
            offset = EditorGUILayout.Vector2Field( "offset", offset );
        }

        void OnSceneGUI(SceneView view)
        {
            if (m_camera == null)
            {
                m_camera = view.camera;
            }
            if (target == null)
            {
                return;
            }
            var area_rect = new Rect( Vector2.zero, view.position.size );
            Handles.BeginGUI();
            GUILayout.BeginArea( area_rect );

            var rect_pos = m_camera.WorldToScreenPoint( target.position );
            Debug.Log( rect_pos );
            rect_pos = view.position.ParentPositionToLocal( rect_pos );
            var rect = new Rect( (Vector2)rect_pos + offset, 100 * new Vector2( 2, 1 ) );
            GUI.Label( rect, "TargetHere!!asDAsdasASDasdASDsadASDASadssdfsdfsdfdf" );
            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}
