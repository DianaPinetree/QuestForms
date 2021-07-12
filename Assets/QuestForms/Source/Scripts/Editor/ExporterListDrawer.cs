using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace QuestForms
{
    [CustomPropertyDrawer(typeof(ExporterListAttribute))]
    public class ExporterListDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ExporterListAttribute exporters = attribute as ExporterListAttribute;

            string[] exporterNames = exporters.Exporters;

            int index = Mathf.Max(0, Array.IndexOf(exporterNames, property.stringValue));

            index = EditorGUI.Popup(position, property.displayName, index, exporterNames);

            property.stringValue = exporterNames[index];
        }
    }
}
