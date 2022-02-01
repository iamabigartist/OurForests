using UnityEngine;
namespace RaycastingTest
{
    public class CameraRayConclusion : MonoBehaviour
    {

        Resolution m_resolution;
        Camera m_camera;
        void Start()
        {
            m_resolution = Screen.currentResolution;
            m_camera = Camera.main;
        }
        void OnDrawGizmos()
        {

            for (float i = 0; i < m_resolution.height; i++)
            {
                for (float j = 0; j < m_resolution.width; j++)
                {
                    Gizmos.DrawRay( m_camera.ScreenPointToRay(
                        new Vector3( j / m_resolution.width, i / m_resolution.height, 0 ) ) );
                }
            }
        }
    }
}
