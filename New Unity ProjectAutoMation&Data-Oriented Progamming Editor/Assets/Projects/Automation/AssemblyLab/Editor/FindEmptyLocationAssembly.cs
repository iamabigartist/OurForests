using System;
using System.Linq;
using MyUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PrototypeUtils;
namespace Automation.AssemblyLab.Editor
{
    public class FindEmptyLocationAssembly : EditorWindow
    {
        [MenuItem( "Labs/Automation.AssemblyLab.Editor/FindEmptyLocationAssembly" )]
        static void ShowWindow()
        {
            var window = GetWindow<FindEmptyLocationAssembly>();
            window.titleContent = new GUIContent( "FindEmptyLocationAssembly" );
            window.Show();
        }



        void OnEnable()
        {
            var assemblies =
                AppDomain.CurrentDomain.GetAssemblies().
                    Where( a => a.Location == string.Empty );
            list = assemblies.Select( a => a.FullName ).ToArray();

        }

        string[] list;
        void CreateGUI()
        {
            rootVisualElement.Set4Padding( 8 );
            rootVisualElement.New( new ListView( list ) );
            // rootVisualElement.Add( new ListView( list ) );
        }
    }
}
