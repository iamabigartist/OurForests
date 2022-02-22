using System;
using System.Linq;
using System.Reflection;
using MyUtils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace UI.ShowAndExecuteFunctionLab
{

    [Serializable]
    public class Set1
    {
        public int a;
        public Set2 s;
    }

    [Serializable]
    public class Set2
    {
        public int b;
        public Set3 s;
    }


    [Serializable]
    public class Set3
    {
        public int c;
    }

    [Serializable]
    public class DynamicShower1 : EditorWindow
    {
        [MenuItem( "Labs/UI.ShowAndExecuteFunctionLab/DynamicShower1" )]
        static void Init()
        {
            var window = GetWindow<DynamicShower1>();
            window.titleContent = new GUIContent( "DynamicShower1" );
            window.Show();
        }

    #region Serialized

        SerializedObject this_;
        SerializedProperty object1_;

    #endregion

        [SerializeReference]
        public object object1;
        Object[] m_objects;
        string[] m_options;
        int option;




        Type result_type;
        string result_state;
        Assembly m_assembly;
        AssemblyName[] referenced_assembles;

        void SearchForSerializableType(string typename) { }

        void OnEnable()
        {

            m_objects = new Object[]
            {
                new Collider(),
                new MeshRenderer()
                // new MethodArgumentsContainer( typeof(ExampleShowedClass).GetMethods( BindingFlags.NonPublic | BindingFlags.Public ) )

            };

            // object1 = new Set1();

            this_ = new SerializedObject( this );
            object1_ = this_.FindProperty( nameof(object1) );
            m_assembly = Assembly.GetExecutingAssembly();
            referenced_assembles = m_assembly.GetReferencedAssemblies();
            m_options = new string[m_objects.Length];
            for (int i = 0; i < m_options.Length; i++)
            {
                m_options[i] = m_objects[i].GetType().Name;
            }
            ActionA = (a, b) => $"{a},{b}";
            // GetMethodInfoFromLambda( (float a,string  b) => test1(1,"" ) );
            // Expression<Action> aaaaa = () => test1( 1, "" );
            // tt = ((MethodCallExpression)(aaaaa).Body).Method;
            tt = (out float a, double b) =>
            {
                a = 1;
                return a;
            };

            m_parameterInfos = tt.GetMethodInfo().GetParameters();
            parameters_string = string.Join( ",", m_parameterInfos.Select( p => $"{(p.IsRetval ? "return" : "")} {p.ParameterType} {p.Name} " ) );
        }



        public delegate float Foo11(out float a, double b);
        Foo11 tt;
        Func<float, string, int> tta;
        Func<int, float, string> ActionA;
        ParameterInfo[] m_parameterInfos;
        string parameters_string;
        Vector2 pos_scroll_view;
        void OnGUI()
        {
            if (GUILayout.Button( "Copy" ))
            {
                var textEditor = new TextEditor
                {
                    text = JsonUtility.ToJson( object1 )
                };
                textEditor.OnFocus();
                textEditor.Copy();
            }

            using (var s = new EditorGUILayout.ScrollViewScope( pos_scroll_view ))
            {
                pos_scroll_view = s.scrollPosition;
                EditorGUILayout.LabelField( $"Current Assembly: {m_assembly.GetName()}" );
                EditorGUILayout.HelpBox( $"Referenced Assembles: " +
                                         $"{string.Join( "\n", referenced_assembles.Select( a => a.Name ) )}",
                    MessageType.Info );
                EditorGUILayout.HelpBox( $"{tt.GetType()}", MessageType.Info );
                EditorGUILayout.HelpBox( parameters_string, MessageType.Info );

                EditorGUILayout.Popup( "Object Type", option, m_options ).Update( ref option, (v, ov) =>
                {
                    object1 = m_objects[v];
                    this_.Update();
                } );


                EditorGUILayout.PropertyField( object1_, true );
                // EditorGUILayout.LabelField( $"managed: {object1_.managedReferenceValue}" );
                // EditorGUILayout.LabelField( $"Object: {object1_.objectReferenceValue}" );
                // EditorGUILayout.LabelField( $"exposed: {object1_.exposedReferenceValue}" );
            }

            this_.ApplyModifiedProperties();

        }
    }
}
