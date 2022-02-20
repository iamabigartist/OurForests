using System;
using UnityEditor;
using UnityEngine;
namespace MyUtils
{
    public static class UIUtils
    {
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
    }
}
