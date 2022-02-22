using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace MyUtils
{
    public static class UIUtils
    {
    #region Layout

        public static Rect GetPart(
            this Rect rect,
            int x_grid_count,
            int y_grid_count,
            int x_i,
            int y_i,
            int x_part_count = 1,
            int y_part_count = 1)
        {
            var x_step = rect.width / x_grid_count;
            var y_step = rect.height / y_grid_count;
            var new_rect = new Rect
            {
                xMin = rect.x + x_i * x_step,
                xMax = rect.x + (x_i + x_part_count) * x_step,
                yMin = rect.y + y_i * y_step,
                yMax = rect.y + (y_i + y_part_count) * y_step
            };
            return new_rect;
        }

        public static void DrawGridUIs(
            Rect position,
            int x_grid_count,
            int y_grid_count,
            Action<Rect, int> element_drawer,
            bool y_first = false)
        {
            int i = 0;
            if (!y_first)
            {
                for (int y = 0; y < y_grid_count; y++)
                {
                    for (int x = 0; x < x_grid_count; x++)
                    {
                        element_drawer( position.GetPart( x_grid_count, y_grid_count, x, y ), i );
                        i++;
                    }
                }
            }
            else
            {
                for (int x = 0; x < x_grid_count; x++)
                {
                    for (int y = 0; y < y_grid_count; y++)
                    {
                        element_drawer( position.GetPart( x_grid_count, y_grid_count, x, y ), i );
                        i++;
                    }
                }
            }

        }

    #endregion


    #region Coordinate

        public static Vector3 WorldToOnGameGUIScreenPosition(this Camera camera, Vector3 world_position)
        {
            var screen_position = camera.WorldToScreenPoint( world_position );
            return new Vector3( screen_position.x, Screen.height - screen_position.y, screen_position.z );
        }
        public static Vector3 WorldToOnSceneViewGUIScreenPosition(this Camera camera, SceneView view, Vector3 world_position)
        {
            var screen_position = camera.WorldToScreenPoint( world_position );

            return new Vector3( screen_position.x, view.position.height - screen_position.y, screen_position.z );
        }
        public static Rect PositionSizeRect(Vector2 position, Vector2 size)
        {
            return new Rect( position - size / 2f, size );
        }
        public static Vector2 ParentPositionToLocal(this Rect r, Vector2 pos)
        {
            return pos - r.min;
        }

    #endregion

    #region EaseCall

        public static T New<T>(this VisualElement root, T visualElement)
            where T : VisualElement
        {
            root.Add( visualElement );
            return visualElement;
        }

    #endregion

    #region StyleSheet

        public static void Set4Padding(this VisualElement v, float padding)
        {
            var s = v.style;
            s.paddingBottom = padding;
            s.paddingTop = padding;
            s.paddingLeft = padding;
            s.paddingRight = padding;
        }

    #endregion

    }
}
