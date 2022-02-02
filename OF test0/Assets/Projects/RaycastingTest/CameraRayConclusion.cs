using System;
using UnityEngine;
namespace RaycastingTest
{
    public class CameraRayConclusion : MonoBehaviour
    {

        Resolution m_resolution;
        Camera m_camera;

        public Vector3 position;
        [SerializeField]
        Vector3 mouse_position;
        void Start()
        {
            m_resolution = Screen.currentResolution;
            m_camera = GetComponent<Camera>();
        }
        void OnDrawGizmos()
        {
            Gizmos.DrawRay( m_camera.ScreenPointToRay( position ) );
        }

        void Update()
        {
            mouse_position = Input.mousePosition;
        }
    }
}
