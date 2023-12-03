using OpenRCT2.Utilities;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace EditorExtensions
{
    /// <summary>
    /// Property drawer for the <see cref="ScriptSelectorAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredDrawer : PropertyDrawer
    {
        static readonly Color _drawerColor = new Color(1, 0.5f, 0.5f, 1f);
        static readonly Color _backgroundColor = new Color(0.35f, 0.1f, 0.1f, 1f);


        /// <summary>
        /// Draws the GUI for a property with a <see cref="ScriptSelectorAttribute"/>.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsNullOrEmpty(property))
            {
                // Red background
                Rect backgroundRect = new Rect(0, position.y - 2, position.width * 2, position.height + 4);
                EditorGUI.DrawRect(backgroundRect, _backgroundColor);

                // Icon
                float size = (EditorGUIUtility.singleLineHeight);
                Rect iconRect = new Rect((position.x - size) + 1, position.y + 1, size, size);
                GUIContent icon = EditorGUIUtility.IconContent("Error", "This property is required and cannot be empty!");
                GUI.Box(iconRect, icon, GUIStyle.none);

                // Property box
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = _drawerColor;
                EditorGUI.PropertyField(position, property, label, includeChildren: true);
                GUI.backgroundColor = oldColor;
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, includeChildren: true);
            }
        }


        /// <summary>
        /// Gets the height of the property.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
        }


        /// <summary>
        /// Returns true if the serialized property is set to null.
        /// </summary>
        bool IsNullOrEmpty(SerializedProperty property)
        {
            return property.propertyType switch
            {
                SerializedPropertyType.String           => string.IsNullOrEmpty(property.stringValue),
                SerializedPropertyType.ExposedReference => (property.exposedReferenceValue == null),
                SerializedPropertyType.ObjectReference  => (property.objectReferenceValue == null),
                SerializedPropertyType.ManagedReference => string.IsNullOrEmpty(property.managedReferenceFieldTypename),

                _ => false,
            };
        }
    }
}
