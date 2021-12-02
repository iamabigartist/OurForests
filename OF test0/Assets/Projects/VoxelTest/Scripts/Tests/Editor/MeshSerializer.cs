using System;
using UnityEditor;
using UnityEngine;
namespace VoxelTest.Tests.Editor
{
    [Serializable]
    public class MeshSerializer : EditorWindow
    {
        [MenuItem( "Tests/MeshSerialiser" )]
        static void Init()
        {
            var window = GetWindow<MeshSerializer>();
            window.titleContent = new GUIContent( "MeshSerialiser" );
            window.Show();
        }

    #region Serialized

        SerializedObject this_;
        SerializedProperty m_mesh_;

    #endregion

        [SerializeField]
        Mesh m_mesh;
        UnityEditor.Editor mesh_editor;


        void OnEnable()
        {

            this_ = new SerializedObject( this );
            m_mesh_ = this_.FindProperty( "m_mesh" );
        }

        void OnGUI()
        {
            if (GUILayout.Button( "Copy" ))
            {
                var _ = new TextEditor();
                _.text = JsonUtility.ToJson( m_mesh );
                _.OnFocus();
                _.Copy();
            }

            EditorGUILayout.PropertyField( m_mesh_ );

            this_.ApplyModifiedProperties();

            if (m_mesh != null)
            {
                mesh_editor = UnityEditor.Editor.CreateEditor( m_mesh );

                mesh_editor.OnInspectorGUI();

            }

            EditorGUILayout.LabelField( $"Triangles: {string.Join( ",", m_mesh.triangles )}" );


            EditorGUILayout.IntField( "UInt16: ", sizeof(ushort) );
            EditorGUILayout.IntField( "uint: ", sizeof(uint) );
            EditorGUILayout.IntField( "Byte: ", sizeof(byte) );
            EditorGUILayout.IntField( "Boolean: ", sizeof(bool) );
            EditorGUILayout.IntField( "bool: ", sizeof(bool) );



        }
    }
}
