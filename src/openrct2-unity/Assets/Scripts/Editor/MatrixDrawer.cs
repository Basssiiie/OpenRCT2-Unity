using System.Collections;
using OpenRCT2.Utilities;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace EditorExtensions
{
    /// <summary>
    /// Property drawer for the <see cref="ScriptSelectorAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(Matrix4x4))]
    public class MatrixDrawer : PropertyDrawer
    {
        // Cache for the drawer, because the same drawer can be used for multiple properties.
        static readonly DrawerCache<bool> _cache = new DrawerCache<bool>();


        const int MatrixSize = 4;

        
        /// <summary>
        /// Draws the GUI for a property with a <see cref="ScriptSelectorAttribute"/>.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            Rect foldoutRect = position;
            foldoutRect.height = height;

            string cacheKey = _cache.Get(property, out bool foldout);
            if ((foldout = EditorGUI.Foldout(foldoutRect, foldout, label, toggleOnLabelClick: true)))
            {
                IEnumerator enumerator = property.GetEnumerator();
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                for (int row = 0; row < MatrixSize; row++)
                {
                    float sx = (position.x);
                    float sy = (position.y + height + spacing);
                    float width = ((position.width / MatrixSize) - spacing);

                    for (int col = 0; col < MatrixSize; col++)
                    {
                        // Draw a single matrix float field.
                        enumerator.MoveNext();
                        SerializedProperty matrixProp = (SerializedProperty)enumerator.Current;
                        float original = matrixProp.floatValue;

                        Rect fieldRect = new Rect
                        (
                            sx + col * (width + spacing),
                            sy + row * (height + spacing),
                            width,
                            height
                        );
                        float output = EditorGUI.FloatField(fieldRect, original);
                        if (output != original)
                        {
                            matrixProp.floatValue = output;
                        }
                    }
                }
            }
            _cache.Set(cacheKey, foldout);
        }


        /// <summary>
        /// Gets the height of the property.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _cache.Get(property, out bool foldout);

            if (!foldout)
                return EditorGUIUtility.singleLineHeight;

            return ((1 + MatrixSize) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing));
        }
    }
}
