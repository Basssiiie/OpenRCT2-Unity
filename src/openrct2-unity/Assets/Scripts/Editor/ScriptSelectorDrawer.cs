using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using OpenRCT2.Utilities;
using UnityEditor;
using UnityEngine;

#nullable enable

namespace EditorExtensions
{
    /// <summary>
    /// Property drawer for the <see cref="ScriptSelectorAttribute"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(ScriptSelectorAttribute))]
    public class ScriptSelectorDrawer : PropertyDrawer
    {
        // Cache for the drawer, because the same drawer can be used for multiple properties.
        static readonly DrawerCache<DrawerData> _cache = new DrawerCache<DrawerData>();


        // The settings for the drawer per property.
        struct DrawerData
        {
            public bool foldout;
            public MonoScript script;
        }


        /// <summary>
        /// Draws the GUI for a property with a <see cref="ScriptSelectorAttribute"/>.
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float fieldSpacing = EditorGUIUtility.standardVerticalSpacing;
            float singleLineHeight = EditorGUIUtility.singleLineHeight;

            string cacheKey = _cache.Get(property, out DrawerData settings);
            Rect rect = position;
            rect.height = singleLineHeight;

            if ((settings.foldout = EditorGUI.Foldout(rect, settings.foldout, label, toggleOnLabelClick: true)))
            {
                rect.y += (singleLineHeight + fieldSpacing);
                EditorGUI.indentLevel++;

                // Script
                if (!TryFindManagedType(property.managedReferenceFullTypename, out Type? managedType))
                    return;

                MonoScript? script;

                if (settings.script != null)
                    script = settings.script;
                else if (TryFindMonoScriptAsset(managedType.Name, out script))
                    settings.script = script;
                else return;

                GUIContent scriptLabel = new GUIContent(fieldInfo.FieldType.Name);
                MonoScript selected = (MonoScript)EditorGUI.ObjectField(rect, scriptLabel, script, typeof(MonoScript), allowSceneObjects: false);

                if (selected != script && ValidateSelectedScript(selected, fieldInfo, out object? instance))
                {
                    property.serializedObject.Update();
                    property.managedReferenceValue = instance;
                    property.serializedObject.ApplyModifiedProperties();
                }

                rect.y += (singleLineHeight + fieldSpacing);

                // Serialized fields
                SerializedProperty end = property.GetEndProperty();
                bool any = property.NextVisible(true);

                while (any && property.propertyPath != end.propertyPath)
                {
                    float propertyHeight = EditorGUI.GetPropertyHeight(property); 
                    rect.height = propertyHeight;

                    EditorGUI.PropertyField(rect, property, label, includeChildren: true);

                    rect.y += (propertyHeight + fieldSpacing);
                    any = property.NextVisible(false);
                }
                EditorGUI.indentLevel--;
            }
            _cache.Set(cacheKey, settings);
        }


        /// <summary>
        /// Gets the height of the property.
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _cache.Get(property, out DrawerData settings);

            if (!settings.foldout)
                return EditorGUIUtility.singleLineHeight;

            float editorHeight = EditorGUI.GetPropertyHeight(property, label, includeChildren: true);
            return (editorHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }


        /// <summary>
        /// Try to find the managed type from the Unity provided type name.
        /// </summary>
        static bool TryFindManagedType(string fullUnityTypeName, [NotNullWhen(true)] out Type? managedType)
        {
            string[] parts = fullUnityTypeName.Split(' ');

            if (parts.Length == 2)
            { 
                managedType = Type.GetType($"{parts[1]}, {parts[0]}");
                return true;
            }

            Debug.LogWarning($"Could not find managed type '{fullUnityTypeName}'!");
            managedType = null;
            return false;
        }


        /// <summary>
        /// Try to find the <see cref="MonoScript"/> asset that contains the specified class.
        /// </summary>
        static bool TryFindMonoScriptAsset(string className, [NotNullWhen(true)] out MonoScript? script)
        {
            string[] assetGuids = AssetDatabase.FindAssets($"{className} t:MonoScript");

            if (assetGuids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuids[0]);

                script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                return true;
            }

            Debug.LogWarning($"Could not find MonoScript asset '{className}'!");
            script = null;
            return false;
        }


        /// <summary>
        /// Validate the selected <see cref="MonoScript"/> to see whether it can
        /// produce a valid instance for the specified property.
        /// </summary>
        static bool ValidateSelectedScript(MonoScript selected, FieldInfo fieldInfo, [NotNullWhen(true)] out object? instance)
        {
            Type classType = selected.GetClass();
            if (classType == null)
            {
                Debug.LogError($"The selected MonoScript '{selected.name}' does not have a matching class!");
                instance = null;
                return false;
            }

            Type fieldType = fieldInfo.FieldType;
            if (!fieldType.IsAssignableFrom(classType))
            { 
                Debug.LogError($"Cannot use script '{classType.Name}'! Only scripts that implement '{fieldType.Name}' are allowed.");
                instance = null;
                return false;
            }

            instance = Activator.CreateInstance(classType);
            return true;
        }
    }
}
