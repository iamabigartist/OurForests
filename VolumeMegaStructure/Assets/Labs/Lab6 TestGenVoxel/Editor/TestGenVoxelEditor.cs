using Unity.Jobs;
using UnityEditor;
using VolumeMegaStructure.Generate.ProceduralMesh.Voxel;
namespace Labs.Lab6_TestGenVoxel.Editor
{
    public class TestGenVoxelEditor : EditorWindow
    {
        [MenuItem( "Labs/Labs.Lab6_TestGenVoxel.Editor/TestGenVoxelEditor" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestGenVoxelEditor>();
            window.titleContent = new("TestGenVoxelEditor");
            window.Show();
        }

        void OnEnable()
        {
            var volume_matrix_scan = new VolumeMatrixScan();
            volume_matrix_scan.Run( 100 );
        }

        void OnGUI() { }
    }
}
