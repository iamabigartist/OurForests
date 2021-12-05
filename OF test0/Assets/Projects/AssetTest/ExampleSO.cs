using System;
using System.Collections.Generic;
using UnityEngine;
namespace AssetTest
{

    [Serializable]
    public class ExampleSO : ScriptableObject
    {
        public int a;
        int b;
        public Dictionary<int, int> A;
        List<int> B;
        public Animal[] Animals;

        void OnEnable()
        {
            a = 1;
            b = 2;
            A = new Dictionary<int, int>();
            A[1] = 1;
            A[100] = 10;
            B = new List<int>();
            B.Add( 11 );
            B.Add( 22 );
            Animals = new Animal[10];
            var animalA = new Animal();
            animalA.Cats = new List<Cat>();
            animalA.Cats.Add( new Cat( "Waibibabu" ) );
            Animals[1] = animalA;
        }
    }


    [Serializable]
    public class Animal
    {
        public List<int> A;
        public List<Cat> Cats;
    }


    [Serializable]
    public class Cat
    {
        public Cat(string name)
        {
            this.name = name;
        }
        public string name;
    }
}
