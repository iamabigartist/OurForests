using System;
using UnityEditor;
using UnityEngine;
using static MyUtils.ContextUtil;
namespace UI.ShowAndExecuteFunctionLab
{

    [Serializable]
    public class Fur
    {
        public Color color;
        public PhysicMaterial material;
        public int heal;
    }

    [Serializable]
    public class Animal
    {
        public Fur fur;
        public int age;
        public Vector3 position;
    }

    [Serializable]
    public class Cat : Animal
    {
        public Animal friend;
    }

    [Serializable]
    public class Dog : Animal
    {
        public Animal up_master;
    }

    public class ExampleShowedClass : EditorWindow
    {
        [MenuItem( "Labs/UI.ShowAndExecuteFunctionLab/ExampleShowedClass" )]
        static void ShowWindow()
        {
            var window = GetWindow<ExampleShowedClass>();
            window.titleContent = new GUIContent( "ExampleShowedClass" );
            window.Show();
        }

        void OnEnable()
        {
            Command1( 1, "" );
        }

        public void Command1(int a, string B)
        {
            SayOutCurrentMethod();
        }

        public void Command2(Vector2 AA, Color cc)
        {
            SayOutCurrentMethod();
        }

        static int Command3(Cat cat, Fur m_fur)
        {
            SayOutCurrentMethod();
            return cat.fur.heal;
        }

        static void SayOutCurrentMethod()
        {
            Debug.Log( GetCallerFrame().GetMethod().Name );
        }
    }
}
