using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace QuestForms.Internal
{
    [CustomPropertyDrawer(typeof(LanguageKeyAttribute))]
    public class LanguageKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            LanguageKeyAttribute lKeys = attribute as LanguageKeyAttribute;

            string[] keyNames = lKeys.Keys;
            if (property.stringValue == null) property.stringValue = keyNames[0];

            // Selected
            int index = Mathf.Max(0, Array.IndexOf(keyNames, property.stringValue));

            index = EditorGUI.Popup(position, property.displayName, index, keyNames);

            property.stringValue = keyNames[index];
        }
    }
}