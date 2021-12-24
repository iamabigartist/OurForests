using UnityEngine;
namespace VolumeTerra.Management
{
    [CreateAssetMenu( fileName = "Cube1", menuName = "", order = 0 )]
    public class CubeConfig : ScriptableObject
    {
        public Material material;
        public Mesh mesh;
    }
}
