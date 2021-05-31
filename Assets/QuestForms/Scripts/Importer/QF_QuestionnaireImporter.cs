using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;

[ScriptedImporter(1, "quest")]
public class QF_QuestionnaireImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        QF_Questionnaire quest = ScriptableObject.CreateInstance<QF_Questionnaire>();

        quest.pages = JsonUtility.FromJson<ImportQuest>(File.ReadAllText(ctx.assetPath)).pages;

        ctx.AddObjectToAsset("Questionnaire", quest);
        ctx.SetMainObject(quest);

        // Do images

    }
}

[System.Serializable]
class ImportQuest 
{
    public Page[] pages;
}