using System;
using UnityEditor;
using UnityEngine;
namespace VolumeMegaStructure.Editor.Util
{
	public class EditorPreviewRenderer
	{
		public PreviewRenderUtility preview_render_utility;

		public EditorPreviewRenderer()
		{
			preview_render_utility = new();
		}

		public Texture GetPreviewTexture(Rect canvas, Action<PreviewRenderUtility> render_commands, GUIStyle preview_background)
		{
			preview_render_utility.BeginPreview(canvas, preview_background);
			render_commands.Invoke(preview_render_utility);
			preview_render_utility.camera.Render();
			var result_texture = preview_render_utility.EndPreview();
			return result_texture;
		}

		public void DrawPreview(float width, float height, Action<PreviewRenderUtility> render_commands)
		{
			var rect = GUILayoutUtility.GetRect(width, height);
			var preview_texture = GetPreviewTexture(rect, render_commands, GUIStyle.none);
			GUI.DrawTexture(rect, preview_texture);
		}

		public void Cleanup()
		{
			preview_render_utility.Cleanup();
		}
	}
}