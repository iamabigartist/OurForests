using UnityEditor;
using UnityEngine;
namespace VoxelTest
{

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

    #endregion

        Mesh m_mesh;

        void OnEnable()
        {
            this_ = new SerializedObject( this );
            m_mesh_ = this_.FindProperty( nameof(m_mesh) );
        }

        void OnGUI()
        {
            EditorGUILayout.PropertyField( m_mesh_ );
            this_.ApplyModifiedProperties();

            if (GUILayout.Button("Generate"))
            {
                Generate_indices();
                Generate_UVs();
            }
        }

        void Generate_indices()
        {

        }

        void Generate_UVs()
        {

        }

    }
}
