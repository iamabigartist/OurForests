using UnityEngine;
namespace Labs.Lab7_TestVertexId
{
    public class TestDrawMeshWithVertexId : MonoBehaviour
    {
        public Mesh mesh;
        public Material material;
        public bool drawMesh = true;
        public int number = 100;

        void Start()
        {
            mesh = new();

        }

        //Use graphic to draw mesh with material
        void Update() { }
    }
}
