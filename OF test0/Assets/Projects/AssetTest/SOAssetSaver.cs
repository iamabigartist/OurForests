using UnityEditor;
using UnityEngine;
namespace AssetTest
{
    public class SOAssetSaver : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var example_so = ScriptableObject.CreateInstance<ExampleSO>();
            AssetDatabase.CreateAsset(example_so,"Assets/GeneratedResults/ExampleSO.mesh");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
