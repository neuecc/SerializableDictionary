#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace SerializableCollections
{
    public abstract class SerializableLookupPropertyDrawer : PropertyDrawer
    {
        bool foldout;
        System.Reflection.BindingFlags reflectionFlags = System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label);

            var target = property.serializedObject.targetObject;
            var lookup = fieldInfo.GetValue(target) as IEnumerable; // ILookup
            if (lookup == null) return height;

            var count = (int)lookup.GetType().GetProperty("Count", reflectionFlags).GetValue(lookup, null);
            return (foldout)
                ? (count + 1) * 17f
                : 17f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var target = property.serializedObject.targetObject;
            var lookup = fieldInfo.GetValue(target) as IEnumerable; // ILookup
            if (lookup == null) return;

            var count = lookup.GetType().GetProperty("Count", reflectionFlags).GetValue(lookup, null);

            foldout = EditorGUI.Foldout(position, foldout, label, true);
            EditorGUI.LabelField(position, label, new GUIContent() { text = "Count:" + count });
            if (foldout)
            {
                // only dump:)
                foreach (IEnumerable item in lookup) // IGrouping
                {
                    var key = item.GetType().GetProperty("Key", reflectionFlags).GetValue(item, null);

                    foreach (var subItem in item)
                    {
                        position = new Rect(position.x, position.y + 17f, position.width, position.height);
                        EditorGUI.LabelField(position, key.ToString(), (subItem == null) ? "null" : subItem.ToString());
                    }

                }
            }
        }
    }
}

#endif