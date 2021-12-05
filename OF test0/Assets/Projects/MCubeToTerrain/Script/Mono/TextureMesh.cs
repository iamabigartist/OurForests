using MCubeToTerrain.Script.Generator;
using UnityEngine;
namespace MCubeToTerrain.Script.Mono
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TextureMesh : MonoBehaviour
    {

        [Tooltip("The scale of a volume")]
        public float scale;
        [Tooltip("The number of volume in each dimension")]
        public int number;
        [Range(0f, 5f)]
        public float iso_value;

        private MeshFilter _filter;

        private MarchingCubeCPUGeneratorTrivial _cubeGenerator;
        private RandomVolumeGenerator _volumeGenerator;

        private void Start()
        {
            this._volumeGenerator = new RandomVolumeGenerator();
            this._cubeGenerator = new MarchingCubeCPUGeneratorTrivial();
            this._filter = GetComponent<MeshFilter>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Generate"))
            {
                this._volumeGenerator.Input(this.number);
                this._volumeGenerator.Output(out VolumeMatrix volume);
                this._cubeGenerator.Input(
                    volume, this.iso_value,
                    Vector3.one * this.scale,
                    transform.position);
                this._cubeGenerator.Output(out Mesh _mesh);
                this._filter.sharedMesh = _mesh;


            }
        }
    }
}