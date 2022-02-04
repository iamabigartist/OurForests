using TMPro;
using UnityEngine;
namespace GPUVoxelTest.Tests
{
    [ExecuteAlways]
    public class PositionHelper : MonoBehaviour
    {
        public float scale;
        public Vector3 offset;
        TMP_Text text_component;
        void Start()
        {
            text_component = GetComponentInChildren<TMP_Text>();
        }

        void Update()
        {
            text_component.text = (scale * transform.localPosition + offset).ToString();
        }
    }
}
