using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static MyUtils.AsyncUtil;
namespace Automation.AsyncLab.Editor
{
    public class TestWaitNull : EditorWindow
    {
        class MyAnimal
        {
            public Color Fur;
        }

        [MenuItem( "Labs/Automation.AsyncLab.Editor/TestWaitNull" )]
        static void ShowWindow()
        {
            var window = GetWindow<TestWaitNull>();
            window.titleContent = new GUIContent( "TestWaitNull" );
            window.Show();
        }

        MyAnimal animal;
        const int NUM = 5;

        void OnEnable()
        {
            Play();
        }

        async void Play()
        {
            animal = null;
            _ = Task.Run( async () =>
            {
                for (int i = 0; i < NUM; i++)
                {
                    await Task.Delay( 1000 );
                    Debug.Log( $"second: {i}" );
                }
                animal = new MyAnimal() { Fur = Color.gray };
            } );
            Debug.Log( $"{(await WaitNull( () => animal )).Fur}" );
        }

        void OnGUI() { }
    }
}
