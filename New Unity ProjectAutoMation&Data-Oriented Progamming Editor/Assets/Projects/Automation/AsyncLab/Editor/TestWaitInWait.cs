using System.Threading;
using System.Threading.Tasks;
using MyUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PrototypeUtils;
namespace Automation.AsyncLab.Editor
{
    public class TestWaitInWait : EditorWindow
    {
        [MenuItem( "Labs/Automation.AsyncLab.Editor/TestWaitInWait" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestWaitInWait>();
            window.titleContent = new GUIContent( "TestWaitInWait" );
            window.Show();
        }

        void OnEnable() { Display(); }

        async void Display()
        {
            await Task.Run( () =>
            {
                Task.Delay( 5 * 1000 ).Wait();
                Debug.Log( "Done" );
            } );
        }

        Label l;

        void CreateGUI()
        {
            rootVisualElement.Set4Padding( 5 );
            l = rootVisualElement.New( new Label() );
        }

        void OnInspectorUpdate()
        {
            l.text = $"{EditorApplication.timeSinceStartup}";
        }
    }
}
