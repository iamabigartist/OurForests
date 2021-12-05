using UnityEditor;
using UnityEngine;
namespace VoxelTest.Scripts.Tests
{
    public class TestTransferWindow : EditorWindow
    {
        [MenuItem( "Tests/TestTransferWindow" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestTransferWindow>();
            window.titleContent = new GUIContent( "TestTransferWindow" );
            window.Show();
        }

        ComputeShader CS;
        Vector3[] array;
        ComputeBuffer buffer;

        (int zxczxc, int asdasd) hh;
        void Awake()
        {
            CS = Resources.Load<ComputeShader>( "TestTransferCS" );

            array = new Vector3[5];
            buffer = new ComputeBuffer( 5, 3 * sizeof(float) );
            CS.SetBuffer( 0, "buffer", buffer );
        }

        void OnInspectorUpdate()
        {
            CS.Dispatch( 0, 1, 1, 1 );
            buffer.GetData( array );
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField( $"array: {string.Join( ",", array )}" );
            EditorGUILayout.LabelField( nameof(hh.asdasd) );
        }
    }
}
