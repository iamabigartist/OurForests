using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using VolumeMegaStructure.Config;
using VolumeMegaStructure.Editor.Util;
namespace VolumeMegaStructure.Editor
{
	[CustomEditor(typeof(BlockConfig))]
	public class BlockConfigInspector : UnityEditor.Editor
	{
		BlockConfig target_;
		EditorPreviewRenderer preview_renderer;
		Light m_light;
		Camera m_camera;
		Mouse m_mouse;

		void OnEnable()
		{
			target_ = (BlockConfig)target;
			m_mouse = Mouse.current;
			InitRender();
			EditorApplication.update += Update;
		}

		void InitRender()
		{
			preview_renderer = new();
			var render_util = preview_renderer.preview_render_utility;
			var show_light_color = Color.white;
			m_camera = render_util.camera;
			m_light = render_util.lights[0];
			m_light.intensity = 1;
			var transform1 = m_light.transform;
			transform1.position = Vector3.up * 5;
			transform1.forward = Vector3.down;
			render_util.ambientColor = show_light_color * 7f / 5f;
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
			preview_renderer.DrawPreview(256f, 256f, util =>
			{
				util.DrawMesh(
					target_.mesh,
					Vector3.zero, Quaternion.identity,
					target_.material, 0);
			});

		}

		void Update()
		{
			if (m_mouse.leftButton.isPressed)
			{
				var cur_delta = Mouse.current.delta.ReadValue();
				var transform = m_camera.transform;
				transform.RotateAround(Vector3.zero, transform.up, cur_delta.x);
				transform.RotateAround(Vector3.zero, transform.right, -cur_delta.y);
				var rotation = transform.rotation;
				rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
				transform.rotation = rotation;
				Repaint();
			}
		}

		void OnDisable()
		{
			preview_renderer.Cleanup();
			EditorApplication.update -= Update;
		}
	}
}