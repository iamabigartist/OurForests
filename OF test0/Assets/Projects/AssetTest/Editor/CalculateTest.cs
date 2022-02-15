using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MUtility;
using UnityEditor;
using UnityEngine;
using VolumeTerra.DataDefinition;
using Random = Unity.Mathematics.Random;
namespace AssetTest
{
    [Serializable]
    public class CalculateTest : EditorWindow
    {
        [MenuItem( "AssetTest/CalculateTest" )]
        static void ShowWindow()
        {
            var window = GetWindow<CalculateTest>();
            window.titleContent = new GUIContent( "CalculateTest" );
            window.Show();
        }

        VolumeMatrix<int> Matrix;
        int index;
        Vector3Int position;
        void OnEnable()
        {
            Matrix = new VolumeMatrix<int>( Vector3Int.one * 10 );
            AA = new Dictionary_S<string, string>();
            this_ = new SerializedObject( this );
            AA_ = this_.FindProperty( nameof(AA) );
            random = new Random();
        }

        [SerializeField]
        Dictionary_S<string, string> AA;

        SerializedObject this_;
        SerializedProperty AA_;

        int thread_num;
        int[] result_array;

        Random random;
        uint a = 0;


        Vector2 scroll_rect;
        void OnGUI()
        {
            using (var scroll_view = new EditorGUILayout.ScrollViewScope( scroll_rect ))
            {
                scroll_rect = scroll_view.scrollPosition;


                //Position Check
                index = EditorGUILayout.IntField( "index:", index );
                if (GUILayout.Button( "Calculate" ))
                {
                    position = Matrix.Position( index );
                }
                EditorGUILayout.LabelField( $"result:{position}" );
                EditorGUILayout.Space( 10 );


                //Serializable Dict Editor
                EditorGUILayout.PropertyField( AA_ );
                this_.ApplyModifiedProperties();
                EditorGUILayout.LabelField( $"keys: {string.Join( ",", AA.Keys )}" );
                EditorGUILayout.LabelField( $"values: {string.Join( ",", AA.Values )}" );
                if (GUILayout.Button( "Change" ))
                {
                    AA.Remove( "QQ" );
                    this_.Update();
                }
                EditorGUILayout.Space( 10 );


                //Thread result check
                thread_num = EditorGUILayout.IntField( "Thread num", thread_num );
                if (GUILayout.Button( "Run" ))
                {
                    a++;
                    result_array = new int[thread_num];
                    long cur_array_index = 1;
                    object cur_array_index_lock = cur_array_index;

                    random.InitState( a );

                    Parallel.For( 0, thread_num, i =>
                    {
                        if (random.NextInt() % 3 != 0)
                        {
                            lock(cur_array_index_lock)
                            {
                                result_array[i] = (int)(long)cur_array_index_lock;
                                cur_array_index_lock = (long)cur_array_index_lock + 1;
                            }
                            //Another method
                            // var cur_index = Interlocked.Increment( ref cur_array_index ) - 1;
                            // result_array[i] = (int)cur_index;
                        }
                        else
                        {
                            result_array[i] = -1;
                        }
                    } );
                }
                EditorGUILayout.LabelField( $"Result: {string.Join( ",", result_array ?? Array.Empty<int>() )}" );

                EditorGUILayout.Space( 10 );


                //List extend count
                if (GUILayout.Button( "Run" ))
                {
                    var list = new List<int>
                    {
                        Capacity = 100
                    };
                    list.AddRange( Enumerable.Repeat( -1, 100 ) );
                }


            }


        }


    }
}
