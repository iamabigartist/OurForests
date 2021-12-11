using System;
using MUtility;
using UnityEditor;
using UnityEngine;
using VolumeTerra.DataDefinition;
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
        }

        [SerializeField]
        Dictionary_S<string, string> AA;

        SerializedObject this_;
        SerializedProperty AA_;

        void OnGUI()
        {
            index = EditorGUILayout.IntField( "index:", index );
            if (GUILayout.Button( "Calculate" ))
            {
                position = Matrix.Position( index );
            }
            EditorGUILayout.LabelField( $"result:{position}" );
            EditorGUILayout.Space();


            EditorGUILayout.PropertyField( AA_ );
            this_.ApplyModifiedProperties();

            EditorGUILayout.LabelField( $"keys: {string.Join( ",", AA.Keys )}" );
            EditorGUILayout.LabelField( $"values: {string.Join( ",", AA.Values )}" );

            if (GUILayout.Button( "Change" ))
            {
                AA.Remove( "QQ" );
                this_.Update();
            }

        }


    }
}
