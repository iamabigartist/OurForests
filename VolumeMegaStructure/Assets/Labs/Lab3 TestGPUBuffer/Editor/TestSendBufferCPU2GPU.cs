using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
namespace Labs.Lab3_TestGPUBuffer.Editor
{
    public class TestSendBufferCPU2GPU : EditorWindow
    {
        [MenuItem( "Labs/Labs.Lab3_TestGPUBuffer.Editor/TestSendBufferCPU2GPU" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestSendBufferCPU2GPU>();
            window.titleContent = new("TestSendBufferCPU2GPU");
            window.Show();
        }


        int count;
        GraphicsBuffer buffer;
        Dictionary<string, string> times;
        Stopwatch stopwatch;

        struct Float5
        {
            public Vector3 pos;
            public Vector2 uv;
        }

        void Main()
        {
            buffer = new(GraphicsBuffer.Target.Vertex, count, sizeof(float) * 5);
            var array = new Float5[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = new()
                {
                    pos = Vector3.back,
                    uv = Vector2.left
                };
            }


            stopwatch.Reset();
            buffer.SetData( array );
            stopwatch.Stop();
            times[$"{nameof(buffer.SetData)}"]=stopwatch.Get
        }

        void OnEnable()
        {
            times = new();
            stopwatch = new();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField( $"Times: {string.Join( ",", times )}" );
        }
    }
}
