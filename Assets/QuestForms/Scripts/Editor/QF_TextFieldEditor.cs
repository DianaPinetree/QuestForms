using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace QuestForms.Internal
{
    [CustomEditor(typeof(QF_TextField))]
    public class QF_TextFieldEditor : Editor
    {
        SerializedProperty showCharacter;
        SerializedProperty characterMin;
        SerializedProperty characterMax;
        SerializedProperty baseLines;

        private void OnEnable()
        {
            showCharacter = serializedObject.FindProperty("showCurrentCharacter");
            characterMin = serializedObject.FindProperty("characterMin");
            characterMax = serializedObject.FindProperty("characterMax");
            baseLines = serializedObject.FindProperty("baseLines");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            showCharacter.boolValue = EditorGUILayout.Toggle("Show Character Count", showCharacter.boolValue);
            EditorGUILayout.PropertyField(baseLines);
            baseLines.intValue = Mathf.Clamp(baseLines.intValue, 2, QF_Rules.MaxTextFieldLines);
            if (baseLines.intValue >= QF_Rules.MaxTextFieldLines) 
            {
                (target as QF_TextField).ShowScrollBar();
            }
            else 
            {
                (target as QF_TextField).HideScrollBar();
            }

            BoldBlock("Min Characters:", characterMin.intValue.ToString());
            BoldBlock("Max Characters:", characterMax.intValue.ToString());

            serializedObject.ApplyModifiedProperties();
        }

        private void BoldBlock(string header, string content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header, EditorStyles.boldLabel);
            GUILayout.Label(content, EditorStyles.wordWrappedLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }

    
}
