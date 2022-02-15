using UnityEngine;
namespace GPUVoxelTest.Tests
{

    public class BufferCubeShow : MonoBehaviour
    {
        Material m_material;
        ComputeBuffer simple_buffer;
        void Start()
        {
            m_material = GetComponent<MeshRenderer>().material;
            simple_buffer = new ComputeBuffer( 2, sizeof(float) );
            simple_buffer.SetData( new[] { 5.123f, 623.12f } );
            m_material.SetBuffer( "volume", simple_buffer );
        }

        void OnDestroy()
        {
            simple_buffer.Release();
        }

    }
}
