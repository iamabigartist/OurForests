using UnityEditor;
namespace UI.ShowAndExecuteFunctionLab
{
    public abstract class EasyPropertyDrawer<T> : PropertyDrawer
    {
        static T GetTarget(SerializedProperty property)
        {
            return (T)(
                property.managedReferenceValue ??
                (property.objectReferenceValue ?
                    property.objectReferenceValue :
                    property.exposedReferenceValue));
        }


    }
}
