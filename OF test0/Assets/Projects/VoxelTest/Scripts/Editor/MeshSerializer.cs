﻿using System;
using System.Collections.Generic;
using System.Linq;
using MUtility;
using UnityEditor;
using UnityEngine;
namespace VoxelTest
{
    [Serializable]
    public class MeshSerializer : EditorWindow
    {
        [MenuItem( "Tests/MeshSerializer" )]
        static void Init()
        {
            var window = GetWindow<MeshSerializer>();
            window.titleContent = new GUIContent( "MeshSerializer" );
            window.Show();
        }

    #region Serialized

        SerializedObject this_;
        SerializedProperty m_mesh_;
        SerializedProperty m_vertices_;
        SerializedProperty m_triangles_;
        SerializedProperty m_uv1_;

    #endregion

        [SerializeField]
        Mesh m_mesh;

        Editor mesh_editor;
        [SerializeField]
        List<Triangle> m_vertices;
        [SerializeField]
        List<int> m_triangles;
        [SerializeField]
        List<Vector2> m_uv1;

        void OnEnable()
        {
            m_mesh = new Mesh();
            m_triangles = new List<int>();
            m_vertices = new List<Triangle>();

            this_ = new SerializedObject( this );
            m_mesh_ = this_.FindProperty( nameof(m_mesh) );
            m_vertices_ = this_.FindProperty( nameof(m_vertices) );
            m_triangles_ = this_.FindProperty( nameof(m_triangles) );
            m_uv1_ = this_.FindProperty( nameof(m_uv1) );
        }

        void Reload()
        {
            m_vertices = m_mesh.vertices.VerticesArrayToTrianglesList();
            m_triangles = m_mesh.triangles.ToList();
            m_uv1 = m_mesh.uv.ToList();
            mesh_editor = Editor.CreateEditor( m_mesh );
            this_.Update();
        }


        Vector2 scroll_position;
        void OnGUI()
        {

            using (var view=new EditorGUILayout.ScrollViewScope(scroll_position))
            {
                scroll_position = view.scrollPosition;

                if (GUILayout.Button( "Copy" ))
                {
                    var textEditor = new TextEditor
                    {
                        text = JsonUtility.ToJson( m_mesh )
                    };
                    textEditor.OnFocus();
                    textEditor.Copy();
                }

                if (GUILayout.Button( "Reload" ))
                {
                    Reload();
                }

                EditorGUILayout.PropertyField( m_mesh_ );

                this_.ApplyModifiedProperties();

                if (mesh_editor != null)
                {
                    mesh_editor.OnInspectorGUI();
                }

                EditorGUILayout.PropertyField( m_triangles_ );
                EditorGUILayout.PropertyField( m_vertices_ );
                EditorGUILayout.PropertyField( m_uv1_ );






                // EditorGUILayout.IntField( "UInt16: ", sizeof(ushort) );
                // EditorGUILayout.IntField( "uint: ", sizeof(uint) );
                // EditorGUILayout.IntField( "Byte: ", sizeof(byte) );
                // EditorGUILayout.IntField( "Boolean: ", sizeof(bool) );
                // EditorGUILayout.IntField( "bool: ", sizeof(bool) );



            }


        }
    }
}
