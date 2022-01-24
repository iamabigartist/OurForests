// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextTypeDrawer.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Slash.Unity.DataBind.Editor.PropertyDrawers
{
    using Slash.Unity.DataBind.Core.Utils;
    using Slash.Unity.DataBind.Editor.Utils;

    using UnityEditor;

    using UnityEngine;

    [CustomPropertyDrawer(typeof(ContextTypeAttribute))]
    public class ContextTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            EditorGUI.BeginChangeCheck();

            // Find all available context classes.
            var contextTypes = ContextTypeCache.ContextTypes;
            var contextTypeIndex = string.IsNullOrEmpty(property.stringValue)
                ? 0
                : contextTypes.FindIndex(
                    contextType => contextType != null && contextType.AssemblyQualifiedName == property.stringValue);
            var newContextTypeIndex = EditorGUI.Popup(
                position,
                label.text,
                contextTypeIndex,
                ContextTypeCache.ContextTypePaths);

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = contextTypes[newContextTypeIndex]?.AssemblyQualifiedName;
            }
            EditorGUI.EndProperty();
        }
    }
}