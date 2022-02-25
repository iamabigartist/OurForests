using UnityEngine;
using static MyUtils.UIUtils;
namespace UI.WorldSpaceUILab
{
    public class Position2ScreenGUI : MonoBehaviour
    {
        public Camera m_camera;
        public Transform target;
        public Rect m_rect;
        void Start() { }
        void OnGUI()
        {
            var rect_pos = m_camera.WorldToOnGameGUIScreenPosition( target.position );
            var rect = PositionSizeRect( rect_pos
                , 100 * new Vector2( 2, 1 ) );

            GUI.Label( rect, "TargetHere!!asDAsdasASDasdASDsadASDASadssdfsdfsdfdf" );
        }


    }
}
