using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

[CustomEditor(typeof(QF_QuestionnaireImporter))]
public class QF_QuestionnaireImporterEditor : ScriptedImporterEditor
{
    private SerializedProperty questionnaire;
    private Editor questEditor;
    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();

        base.ApplyRevertGUI();
    }
}
