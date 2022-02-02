using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace VoxelTest
{

    [Serializable]
    /// <summary>
    ///     Read a prototype mesh and generate the 24 rotated version uv and surface indices of it.
    /// </summary>
    public class CubeTextureInfoGenerator : EditorWindow
    {



        [MenuItem( "VoxelTest/CubeTextureInfoGenerator" )]
        static void ShowWindow()
        {
            var window = GetWindow<CubeTextureInfoGenerator>();
            window.titleContent = new GUIContent( "CubeTextureInfoGenerator" );
            window.Show();
        }

    #region Serialized

        SerializedObject this_;
        SerializedProperty m_mesh_;
        SerializedProperty m_list_;

    #endregion

        [SerializeField]
        Mesh m_mesh;

        [SerializeField]
        List<Vector3> m_list;

        void OnEnable()
        {
            m_list = new List<Vector3>()
            {
                Vector3.back,
                Vector3.down,
                Vector3.left
            };
            m_mesh = new Mesh();
            this_ = new SerializedObject( this );
            m_mesh_ = this_.FindProperty( nameof(m_mesh) );
            m_list_ = this_.FindProperty( nameof(m_list) );
        }

        int up_index;
        int forward_index;
        string quaternion_quad;
        string group_rotate_quad;
        void OnGUI()
        {
            EditorGUILayout.PropertyField( m_mesh_ );
            EditorGUILayout.PropertyField( m_list_ );
            this_.ApplyModifiedProperties();


            if (GUILayout.Button( "Generate" ))
            {
                Generate_indices();
            }

            using (var hor = new EditorGUILayout.HorizontalScope())
            {
                up_index = EditorGUILayout.IntField( nameof(up_index), up_index );
                forward_index = EditorGUILayout.IntField( nameof(forward_index), forward_index );
            }

            if (GUILayout.Button( "Reload" )) { }

            // EditorGUILayout.LabelField( $"quaternion quad: " );




        }

        void Generate_indices()
        {
        }



    }

}
