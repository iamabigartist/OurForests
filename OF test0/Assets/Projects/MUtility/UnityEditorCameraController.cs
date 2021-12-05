using System;
using UnityEngine;
namespace MUtility
{
    public class UnityEditorCameraController : MonoBehaviour
    {

    #region Config

        [Tooltip( "WASD Move" )][Range( 0, 6 )]
        public float MoveSpeedLog10 = 2f;
        float move_speed => Mathf.Pow( 10, MoveSpeedLog10 ) * speed_ratio;

        [Tooltip( "MMB Drag" )][Range( 0, 6 )]
        public float DragSpeedLog10 = 2f;
        float drag_speed => Mathf.Pow( 10, DragSpeedLog10 ) * speed_ratio;

        [Tooltip( "RMB Rotate" )][Range( 0, 6 )]
        public float RotateSensitiveLog10 = 0.7f;
        float rotate_sensitive => Mathf.Pow( 10, RotateSensitiveLog10 );

        [Tooltip( "Use the mouse scroll wheel to change the speed of moving and dragging" )][Range( 0, 6 )]
        public float ScrollWheelSensitiveLog10 = 1;
        float scroll_sensitive => Mathf.Pow( 10, ScrollWheelSensitiveLog10 );

        [Tooltip( "Flip the y axis rotation of input" )]
        public bool y_flip;
        [Tooltip( "Flip the x axis rotation of input" )]
        public bool x_flip;

        [SerializeField] float speed_ratio = 1;

    #endregion

    #region Component

        Transform target_t;
        Transform t;

    #endregion

    #region Operation

        static Vector2 mouse_move => new Vector2( Input.GetAxis( "Mouse X" ), Input.GetAxis( "Mouse Y" ) );
        static Vector2 wasd_move => new Vector2( Input.GetAxis( "Horizontal" ), Input.GetAxis( "Vertical" ) );
        static float mouse_wheel_move => Input.GetAxis( "Mouse ScrollWheel" );

        [Flags]
        enum InputMode
        {
            Drag,
            Rotate,
            None
        }
        (
            InputMode input_mode,
            Vector2 move,
            Vector2 drag,
            Vector2 rotate,
            float speed_change
            )
            user_input = (InputMode.None, Vector2.zero, Vector2.zero, Vector2.zero, 1f);

        void check_input_mode()
        {
            if (user_input.input_mode == InputMode.None)
            {
                if (Input.GetKeyDown( KeyCode.Mouse1 ))
                {
                    user_input.input_mode = InputMode.Rotate;
                }
                else if (Input.GetKeyDown( KeyCode.Mouse2 ))
                {
                    user_input.input_mode = InputMode.Drag;
                }
            }
            else
            {
                if (
                    Input.GetKeyUp( KeyCode.Mouse1 ) && user_input.input_mode == InputMode.Rotate ||
                    Input.GetKeyUp( KeyCode.Mouse2 ) && user_input.input_mode == InputMode.Drag)
                {
                    user_input.input_mode = InputMode.None;
                }
            }

        }

        void refresh_input()
        {
            user_input.move =
                move_speed *
                (wasd_move +
                 (user_input.input_mode == InputMode.None ?
                     new Vector2( 0, mouse_wheel_move * 10 )
                     : Vector2.zero));

            user_input.drag =
                user_input.input_mode == InputMode.Drag ?
                    mouse_move * drag_speed
                    : Vector2.zero;

            user_input.rotate =
                user_input.input_mode == InputMode.Rotate ?
                    mouse_move * rotate_sensitive *
                    new Vector2( x_flip ? -1 : 1, y_flip ? -1 : 1 )
                    : Vector2.zero;

            user_input.speed_change =
                user_input.input_mode == InputMode.Rotate ?
                    mouse_wheel_move : 0;

        }

        void apply_input()
        {
            speed_ratio +=
                user_input.speed_change * scroll_sensitive *
                Mathf.Pow( 10, Mathf.CeilToInt( Mathf.Log10( speed_ratio ) ) - 1.5f );
            if (speed_ratio < 0)
            {
                speed_ratio = 1e-10f;
            }

            target_t.Translate( Time.deltaTime * new Vector3( user_input.move.x, 0, user_input.move.y ), target_t );

            var e = target_t.rotation.eulerAngles;
            target_t.rotation = Quaternion.Euler( e + new Vector3( -user_input.rotate.y, user_input.rotate.x, 0 ) );

            target_t.Translate( Time.deltaTime * new Vector3( -user_input.drag.x, -user_input.drag.y, 0 ), target_t );

            t.position = Vector3.Lerp( t.position, target_t.position, 0.3f );
            t.rotation = Quaternion.Lerp( t.rotation, target_t.rotation, 0.3f );
        }

    #endregion

    #region UnityEventHandler

        void Start()
        {
            target_t = new GameObject().transform;
            target_t.parent = transform.parent;
            t = transform;
            target_t.position = t.position;
            target_t.rotation = t.rotation;
        }

        void Update()
        {
            // Debug.Log($"x: {Input.GetAxis("Mouse X")}, y: {Input.GetAxis("Mouse Y")}");
            // Debug.Log($"h: {Input.GetAxis("Horizontal")}, v: {Input.GetAxis("Vertical")}");
            check_input_mode();
            refresh_input();
            apply_input();
        }


        float box_alpha = 1;
        void OnGUI()
        {

            var window_width = Screen.width;
            var box_width = 100;
            var window_height = Screen.height;
            var box_height = 25;

            if (Input.mouseScrollDelta.magnitude != 0)
            {
                box_alpha = 1;
            }
            else
            {
                box_alpha =
                    Mathf.Lerp( box_alpha, 0,
                        Mathf.Lerp( 0.01f, 0.000001f,
                            Mathf.Pow( box_alpha, 1 / 3f ) ) );
            }
            GUI.color = new Color( 1, 1, 1, box_alpha );
            GUI.Box( new Rect(
                    window_width / 2f - box_width / 2f,
                    window_height / 2f - box_height,
                    box_width, box_height ),
                $"{Math.Round( speed_ratio, 6 )}x" );
            GUI.color = Color.white;
        }

    #endregion

    }
}
