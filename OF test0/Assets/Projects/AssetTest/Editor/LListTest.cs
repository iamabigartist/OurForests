using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace AssetTest
{
    public class LListTest : EditorWindow
    {
        [MenuItem( "AssetTest/LListTest" )]
        static void ShowWindow()
        {
            var window = GetWindow<LListTest>();
            window.titleContent = new GUIContent( "LListTest" );
            window.Show();
        }


        LinkedList<int> test_ll;
        int[] test_array;
        float record1;


        int len;

        void OnEnable()
        {
            test_ll = new LinkedList<int>();
            record1 = 0;
        }

        void OnGUI()
        {
            len = EditorGUILayout.IntField( "List Length", len );
            if (GUILayout.Button( "Run" ))
            {
                InitList();
                TurnList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                AddList();
                stopwatch.Stop();
                record1 = stopwatch.ElapsedTicks / 10000f;
            }
            EditorGUILayout.LabelField( $"Record: {record1} ms" );
        }


        void InitList()
        {
            test_ll.Clear();
            for (int i = 0; i < len; i++)
            {
                test_ll.AddLast( 1 );
            }
        }


        void TurnList()
        {
            test_array = test_ll.ToArray();
        }

        void AddList()
        {
            for (int i = 0; i < test_array.Length; i++)
            {
                test_array[i] += 1;
            }
        }
    }
}
