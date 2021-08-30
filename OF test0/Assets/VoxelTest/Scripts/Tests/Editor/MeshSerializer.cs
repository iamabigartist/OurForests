using System;
using UnityEditor;
using UnityEngine;
namespace Tests
{
    [Serializable]
    public class MeshSerializer : EditorWindow
    {
        [MenuItem("Tests/MeshSerialiser")]
        private static void Init()
        {
            var window = GetWindow<MeshSerializer>();
            window.titleContent = new GUIContent("MeshSerialiser");
            window.Show();
        }

#region Serialized

        private SerializedObject this_;
        private SerializedProperty m_mesh_;

  #endregion

        [SerializeField] private Mesh m_mesh;
        private Editor mesh_editor;


        private void OnEnable()
        {
            this.m_mesh = new Mesh();

            this.this_ = new SerializedObject(this);
            this.m_mesh_ = this.this_.FindProperty("m_mesh");
        }

        private Vector2 pos_scroll_view;
        private void OnGUI()
        {
            if (GUILayout.Button("Copy"))
            {
                var _ = new TextEditor();
                _.text = JsonUtility.ToJson(this.m_mesh);
                _.OnFocus();
                _.Copy();
            }

            using (var s = new EditorGUILayout.ScrollViewScope(this.pos_scroll_view))
            {
                this.pos_scroll_view = s.scrollPosition;
                EditorGUILayout.PropertyField(this.m_mesh_);
            }

            if (this.m_mesh != null)
            {
                if (this.mesh_editor == null) this.mesh_editor = Editor.CreateEditor(this.m_mesh);

                this.mesh_editor.OnInspectorGUI();

            }


            this.this_.ApplyModifiedProperties();

            EditorGUILayout.IntField("UInt16: ",sizeof(UInt16));
            EditorGUILayout.IntField("uint: ",sizeof(uint));
            EditorGUILayout.IntField("Byte: ",sizeof(Byte));
            EditorGUILayout.IntField("Boolean: ",sizeof(Boolean));
            EditorGUILayout.IntField("bool: ",sizeof(bool));



        }
    }
}