using System;
using UnityEngine;
using UnityEngine.UIElements;
namespace VolumeMegaStructure.Util
{
    /// <summary>
    ///     Imitate the IMGUI syntax for the UIElement code
    /// </summary>
    public class UIBuilder
    {
        public event Action OnUpdate;
        public VisualElement root;

        public T Add<T>(T visualElement)
            where T : VisualElement
        {
            root.Add( visualElement );

            return visualElement;
        }

        public TVisualElement Add<TVisualElement, TValue>(
            TVisualElement visualElement,
            EventCallback<ChangeEvent<TValue>> callback)
            where TVisualElement : VisualElement, INotifyValueChanged<TValue>
        {
            root.Add( visualElement );
            visualElement.RegisterValueChangedCallback( callback );

            return visualElement;
        }

        public TFieldVisualElement Add<TFieldVisualElement, TValue>(
            TFieldVisualElement field,
            Func<TValue> get)
            where TFieldVisualElement : BaseField<TValue>
        {
            root.Add( field );
            OnUpdate += () =>
            {
                UpdateValue( () => field.value, get, value => field.value = value );
            };

            return field;
        }

        public static void UpdateValue<T>(Func<T> get_from, Func<T> get_to, Action<T> set)
        {
            if (!get_from().Equals( get_to() ))
            {
                set( get_to() );
            }
        }

        int a = 1;

        void A()
        {
            var text_field = Add<TextField, string>( new("asd"),
                e => Debug.Log( e.newValue ) );
            string aa = ";";
            aa = "saas";
            var int_field = Add<IntegerField, int>( new(), () => a );
        }



        public void Bind(TextField field, Func<string> get)
        {
            OnUpdate += () =>
            {
                UpdateValue( () => field.value, get, s => field.value = s );
            };
        }
    }
}
