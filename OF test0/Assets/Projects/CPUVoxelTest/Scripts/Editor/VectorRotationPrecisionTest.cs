using System.Linq;
using MUtility;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace CPUVoxelTest.Scripts.Editor
{
    public class VectorRotationPrecisionTest : EditorWindow
    {
        [MenuItem( "CPUVoxelTest.Scripts.Editor/VectorRotationPrecisionTest" )]
        static void ShowWindow()
        {
            var window = GetWindow<VectorRotationPrecisionTest>();
            window.titleContent = new GUIContent( "VectorRotationPrecisionTest" );
            window.Show();
        }

        Vector3[] rotate_axis = { Vector3.right, Vector3.up, Vector3.forward };
        int[] rotate_positive = { 1, -1 };
        string[] rotate_axis_choices =
        {
            "x", "y", "z"
        };
        string[] rotate_positive_choices =
        {
            "+", "-"
        };

        public Vector3 cur_vector = Vector3.forward;
        public int cur_axis;
        public int cur_positive;

        Vector3Field m_vector_field;

        void RotateOnce()
        {
            var axis = rotate_axis[1];
            var cur_rotation =
                Quaternion.AngleAxis(
                    rotate_positive[cur_positive] * 90f,
                    rotate_axis[cur_axis] );
            cur_vector = cur_rotation * cur_vector;
        }

        void PrecisiateVector()
        {
            cur_vector = cur_vector.Round();
        }

        void CreateGUI()
        {
            m_vector_field = new Vector3Field( nameof(cur_vector) )
            {
                value = cur_vector
            };
            var axis_dropdown = new PopupField<int>(
                nameof(rotate_axis), Enumerable.Range( 0, 3 ).ToList(), 0,
                i => rotate_axis_choices[i],
                i => rotate_axis_choices[i] );
            axis_dropdown.RegisterCallback<ChangeEvent<int>, VectorRotationPrecisionTest>(
                (e, t) =>
                {
                    t.cur_axis = e.newValue;
                }, this );
            var positive_dropdown = new PopupField<int>(
                nameof(rotate_positive), Enumerable.Range( 0, 2 ).ToList(), 0,
                i => rotate_positive_choices[i],
                i => rotate_positive_choices[i] );
            positive_dropdown.RegisterCallback<ChangeEvent<int>, VectorRotationPrecisionTest>(
                (e, t) =>
                {
                    t.cur_positive = e.newValue;
                }, this );

            var rotate_once_button = new Button( () =>
            {
                RotateOnce();
                PrecisiateVector();
                m_vector_field.value = cur_vector;
            } )
            {
                text = nameof(RotateOnce)
            };

            rootVisualElement.Add( m_vector_field );
            rootVisualElement.Add( axis_dropdown );
            rootVisualElement.Add( positive_dropdown );
            rootVisualElement.Add( rotate_once_button );

        }
    }
}
