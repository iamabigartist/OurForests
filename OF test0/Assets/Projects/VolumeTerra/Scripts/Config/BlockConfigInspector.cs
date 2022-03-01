using MUtility;
using PrototypeUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
namespace VolumeTerra.Config
{
    [CustomEditor( typeof(BlockConfig) )]
    public class BlockConfigInspector : Editor
    {
        BlockConfig target_;
        PreviewRenderUtility m_renderUtility;
        Light m_light;
        Camera m_camera;
        Mouse m_mouse;
        Rect preview_canvas;

        void OnEnable()
        {
            target_ = (BlockConfig)target;
            m_mouse = Mouse.current;
            InitRenderUtility();
            EditorApplication.update += Update;
        }

        void InitRenderUtility()
        {
            var show_light_color = Color.white;
            m_renderUtility = new PreviewRenderUtility();
            m_camera = m_renderUtility.camera;
            m_light = m_renderUtility.lights[0];
            m_light.intensity = 1;
            var transform1 = m_light.transform;
            transform1.position = Vector3.up * 5;
            transform1.forward = Vector3.down;
            m_renderUtility.ambientColor = show_light_color * 7f / 5f;
            m_camera.farClipPlane = 100000f;
            m_camera.nearClipPlane = 0.000001f;
            // m_camera.orthographic = true;
            var transform = m_camera.transform;
            transform.position = Vector3.back * 10f;
            transform.forward = Vector3.forward;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            preview_canvas = GUILayoutUtility.GetRect( 256f, 256f );
            m_renderUtility.BeginPreview( preview_canvas, GUIStyle.none );
            m_renderUtility.DrawMesh(
                target_.mesh,
                Vector3.zero, Quaternion.identity,
                target_.material, 0 );
            m_renderUtility.camera.Render();
            var result_texture = m_renderUtility.EndPreview();
            GUI.DrawTexture( preview_canvas, result_texture );
        }

        void Update()
        {
            if (m_mouse.leftButton.isPressed)
            {
                var cur_delta = Mouse.current.delta.ReadValue();
                var transform = m_camera.transform;
                transform.RotateAround( Vector3.zero, transform.up, cur_delta.x );
                transform.RotateAround( Vector3.zero, transform.right, -cur_delta.y );
                var rotation = transform.rotation;
                rotation = new Vector3( rotation.eulerAngles.x, rotation.eulerAngles.y, 0 ).ToQuaternion();
                transform.rotation = rotation;
                Repaint();
            }
        }

        void OnDisable()
        {
            m_renderUtility.Cleanup();
            EditorApplication.update -= Update;
        }
    }
}
