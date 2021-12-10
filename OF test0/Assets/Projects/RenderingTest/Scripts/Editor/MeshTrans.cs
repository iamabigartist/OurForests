using System.Linq;
using UnityEditor;
using UnityEngine;
namespace RenderingTest
{
    public class MeshTrans : EditorWindow
    {
        [MenuItem( "RenderingTest.Editor/MeshTrans" )]
        static void ShowWindow()
        {
            var window = GetWindow<MeshTrans>();
            window.titleContent = new GUIContent( "MeshTrans" );
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
            m_mesh = new Mesh();

            this_ = new SerializedObject( this );
            m_mesh_ = this_.FindProperty( "m_mesh" );

            generated_path = "Assets/GeneratedResults/";

        }

        string generated_path;

        void OnGUI()
        {
            EditorGUILayout.PropertyField( m_mesh_ );

            this_.ApplyModifiedProperties();

            if (GUILayout.Button( "turn to pure vertices mesh" ))
            {

                AssetDatabase.CreateAsset( TransMesh( m_mesh ), generated_path + $"{m_mesh.name}" + ".mesh" );
            }
            // EditorGUILayout.LabelField( $"Result Path:{generated_path}" );

        }

        Mesh TransMesh(Mesh m)
        {
            var v_indices = m.triangles;
            var n_vertices = (from i in v_indices select m.vertices[i]).ToArray();
            var n_uv = (from i in v_indices select m.uv[i]).ToArray();

            var new_m = new Mesh
            {
                vertices = n_vertices,
                uv = n_uv,
                triangles = Enumerable.Range( 0, n_vertices.Length ).ToArray()
            };

            new_m.RecalculateBounds();
            new_m.RecalculateNormals();
            new_m.RecalculateTangents();
            return new_m;
        }
    }

}
