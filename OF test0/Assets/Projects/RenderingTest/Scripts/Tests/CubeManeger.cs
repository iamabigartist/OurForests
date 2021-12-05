using UnityEngine;
namespace RenderingTest.Scripts.Tests
{
    public class CubeManeger : MonoBehaviour
    {
        [SerializeField]
        int cube_count;
        [Range( 0, 1f )]
        public float rendering_ratio;
        public GameObject m_cube_prefab;

        void GenerateAllCube()
        {
            for (int i = 0; i < cube_count; i++)
            {
                Instantiate( m_cube_prefab,
                    new Vector3(
                        Random.Range( 0, 10f ),
                        Random.Range( 0, 10f ),
                        Random.Range( 0, 10f ) ),
                    Quaternion.identity, transform );
            }
        }

        void RefreshAllCubeRenderer()
        {
            var cube_renderer = transform.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < cube_renderer.Length; i++)
            {
                cube_renderer[i].enabled = Random.Range( 0f, 1f ) < rendering_ratio;
            }
        }

        void Start()
        {
            GenerateAllCube();
        }

        void OnValidate()
        {
            RefreshAllCubeRenderer();
        }

    }
}
