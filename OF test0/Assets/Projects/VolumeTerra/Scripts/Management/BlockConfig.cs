using UnityEngine;
namespace VolumeTerra.Management
{
    [CreateAssetMenu( fileName = "Block1", menuName = "", order = 0 )]
    public class BlockConfig : ScriptableObject
    {
        public Material material;
        public Mesh mesh;
    }
}
