using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine.UIElements;
using QuestForms;

namespace QuestForms.Internal
{
    [CustomEditor(typeof(QF_QuestionnaireImporter))]
    public class QF_ImporterEditor : ScriptedImporterEditor
    {
        SerializedProperty questionnaire;
        public override void OnEnable()
        {
            base.OnEnable();

            questionnaire = serializedObject.FindProperty("quest");
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Select Questionnaire Asset")) 
            {
                Selection.activeObject = questionnaire.objectReferenceValue;
            }

            ApplyRevertGUI();
        }
    }
}
