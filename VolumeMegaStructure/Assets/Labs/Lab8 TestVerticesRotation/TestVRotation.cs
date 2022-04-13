using PrototypeUtils;
using Unity.Mathematics;
using UnityEngine;
namespace Labs.Lab8_TestVerticesRotation
{
    public class TestVRotation : MonoBehaviour
    {
        readonly int2[] uv_list =
        {
            (0, 0).i2(),
            (1, 0).i2(),
            (1, 1).i2(),
            (0, 1).i2()
        };
        /// <summary>
        ///     Construct a quad with 4 vertices, each with a uv in the const list.
        /// </summary>
        Mesh InitQuad()
        {

        }
        void Start() { }

        void Update() { }
    }
}
