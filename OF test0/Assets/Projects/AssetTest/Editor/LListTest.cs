using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MUtility;
using PrototypeUtils;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
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

        LinkedList<List<int>> test_lll;
        List<int[]> test_llll;

        int len;

        void OnEnable()
        {
            test_ll = new LinkedList<int>();
            test_lll = new LinkedList<List<int>>();
            test_llll = new List<int[]>();
            record1 = 0;
        }

        void OnGUI()
        {
            len = EditorGUILayout.IntField( "List Length", len );
            if (GUILayout.Button( "Run" ))
            {


                InitLLList();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                test_llll.GetFlattenCopyPositions();
                stopwatch.Stop();
                record1 = stopwatch.ElapsedTicks / 10000f;

                stopwatch.Restart();
                TurnLLList();
                stopwatch.Stop();
                record1 -= stopwatch.ElapsedTicks / 10000f;

                var AQAA = new List<int>() { 1, 2, 3 };
                Debug.Log( AQAA.IndexOf( 4 ) );


            }
            EditorGUILayout.LabelField( $"Record: {record1} ms" );

            var a = new int[23];
        }


        // void IterateThroughLink()
        // {
        //     const int thread_num = 10;
        //     int node_count_thread = Mathf.CeilToInt( (float)test_lll.Count / thread_num );
        //     for (int i = 0; i < thread_num; i++)
        //     {
        //         LinkedListNode<int> cur_node=
        //         for (int j = 0; j < node_count_thread; j++)
        //         {
        //
        //         }
        //     }
        // }

        void InitList()
        {
            test_ll.Clear();
            for (int i = 0; i < len; i++)
            {
                test_ll.AddLast( 1 );
            }
        }


        void InitLList()
        {
            test_lll.Clear();

            var list = new List<int>();
            for (int i = 0; i < 24; i++)
            {
                list.Add( 1 );
            }

            for (int i = 0; i < Mathf.CeilToInt( len / 24f ); i++)
            {
                test_lll.AddLast( list );
            }
        }

        List<int> test_llll_start_indices;

        void InitLLList()
        {
            test_llll = new List<int[]>();

            var array = new int[24];
            for (int i = 0; i < 24; i++)
            {
                array[i] = 1;
            }

            int count = Mathf.CeilToInt( len / 24f );
            test_llll.Capacity = count;
            test_llll_start_indices = new List<int>
            {
                Capacity = count
            };
            int cur_sum = 0;
            for (int i = 0; i < count; i++)
            {
                test_llll.Add( array.Copy() );
                test_llll_start_indices.Add( cur_sum );
                cur_sum += test_llll[i].Length;
            }

        }


        void TurnList()
        {
            test_array = test_ll.ToArray();
        }

        void TurnLList()
        {
            test_array = test_lll.ToFlattenedArray();
        }

        void TurnLLList()
        {
            test_array = test_llll.ToFlattenedArrayParallel( test_llll.GetFlattenCopyPositions() );
        }

        void TurnLLListMany()
        {
            test_array = test_llll.SelectMany( (i) => i ).ToArray();
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
