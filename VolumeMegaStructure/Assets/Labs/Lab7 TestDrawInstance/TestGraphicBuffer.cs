using UnityEngine;
namespace Labs.Lab7_TestDrawInstance
{
    public class TestGraphicBuffer : MonoBehaviour
    {
        GraphicsBuffer graphics_buffer;
        Mesh mesh;

        void Start()
        {
            graphics_buffer = new(GraphicsBuffer.Target.Vertex, 100, 4);
        }
    }
}
