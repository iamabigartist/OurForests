using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace MUtility.Testers
{
    public class ArrayRangeSliceTester : EditorWindow
    {
        [MenuItem( "MUtility.Testers/ArrayRangeSliceTester" )]
        static void ShowWindow()
        {
            var window = GetWindow<ArrayRangeSliceTester>();
            window.titleContent = new GUIContent( "ArrayRangeSliceTester" );
            window.Show();
        }

        int[] m_ints;
        int[] a;
        int[] b;

        void OnEnable()
        {
            m_ints = (..10).ToArray();
            a = m_ints[2..4];
            a[0] = 100;
            b = m_ints[..];
            b[0] = 1000;
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField( m_ints.ToMString() );
            EditorGUILayout.LabelField( a.ToMString() );
            EditorGUILayout.LabelField( b.ToMString() );

        }
    }
}
