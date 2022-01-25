using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using MyUtils;
using UnityEditor;
using UnityEngine;
namespace UI.ShowAndExecuteFunctionLab
{

    [CustomPropertyDrawer( typeof(MethodArgumentsContainer) )]
    public class MethodArgumentsContainerDrawer : PropertyDrawer
    {
        public const int arg_height = 20;

        public int i;
        public int args_count;
        public string[] method_names;
        int GetArgsCount(SerializedProperty property)
        {
            return property.
                FindPropertyRelative( "method_arguments" ).
                FindPropertyRelative( "args" ).
                FindPropertyRelative( "Array.size" ).intValue;
        }

        string[] GetMethodNameList(SerializedProperty property)
        {
            var method_count = property.
                FindPropertyRelative( "method_list" ).
                FindPropertyRelative( "Array.size" ).intValue;
            var name_list = new string[method_count];
            for (int j = 0; j < method_count; j++)
            {
                name_list[j] = property.FindPropertyRelative( "method_list" ).
                    GetArrayElementAtIndex( j ).FindPropertyRelative( "Name" ).stringValue;
            }
            return name_list;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            args_count = GetArgsCount( property );
            method_names = GetMethodNameList( property );
            EditorGUI.BeginProperty( position, label, property );
            EditorGUI.LabelField( position.GetPart( 1, args_count + 2, 0, 0 ), label );
            EditorGUI.Popup(
                    position.GetPart( 1, args_count + 2, 0, 1 ),
                    i, GetMethodNameList( property ) ).
                Update( ref i, (v, ov) =>
                {
                    property.FindPropertyRelative( "Option" ).intValue = v;
                } );
            EditorGUI.PropertyField( position.GetPart(
                    1, args_count + 2,
                    0, 2, 1, args_count ),
                property.FindPropertyRelative( "method_arguments" ).FindPropertyRelative( "args" ), true );
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (GetArgsCount( property ) + 2) * arg_height;
        }
    }

    [Serializable]
    public class MethodArgumentsContainer
    {
        public SerializedMethodArguments method_arguments;
        [SerializeReference]
        public MethodInfo[] method_list;
        int option;
        public int Option
        {
            get => option;
            set
            {
                if (option != value)
                {
                    option = value;
                    method_arguments = new SerializedMethodArguments( method_list[option] );
                }
            }
        }

        public MethodArgumentsContainer(MethodInfo[] method_list)
        {
            Trace.Assert( method_list != null && method_list.Length != 0 );
            this.method_list = method_list;
            Option = 0;
        }
    }

    [Serializable]
    public class SerializedMethodArguments
    {
        public MethodInfo method_info;
        [SerializeReference]
        public object[] args;

        public SerializedMethodArguments(MethodInfo method_info)
        {
            this.method_info = method_info;
            var parameters = method_info.GetParameters();
            args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var type = parameters[i].ParameterType;
                args[i] = Activator.CreateInstance( type );
            }
        }

        public object Invoke(Delegate @delegate)
        {
            return @delegate.DynamicInvoke( args );
        }
    }

    public class MethodsInvoker
    {
        public Dictionary<MethodInfo, Delegate> method_delegates;
        public MethodsInvoker()
        {
            method_delegates = new Dictionary<MethodInfo, Delegate>();
        }
        public MethodsInvoker(object obj) : this()
        {
            var type = obj.GetType();
            AddDelegates( type.GetDelegates( obj ) );
        }

        public void AddDelegates((MethodInfo[] m, Delegate[] d) tuple)
        {
            var (m, d) = tuple;
            Trace.Assert( m.Length == d.Length );
            for (int i = 0; i < m.Length; i++)
            {
                method_delegates[m[i]] = d[i];
            }
        }

        public void Invoke(SerializedMethodArguments args_container)
        {
            args_container.Invoke( method_delegates[args_container.method_info] );
        }
    }



}
