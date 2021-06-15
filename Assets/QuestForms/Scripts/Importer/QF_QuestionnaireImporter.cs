using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using QuestForms;

namespace QuestForms.Internal
{
    [ScriptedImporter(2, "quest")]
    public class QF_QuestionnaireImporter : ScriptedImporter
    {
        public QF_Questionnaire quest;
        public Page[] pages;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (quest == null)
                quest = ScriptableObject.CreateInstance<QF_Questionnaire>();
            
            quest.pages = JsonUtility.FromJson<ImportQuest>(File.ReadAllText(ctx.assetPath)).pages;
            pages = quest.pages;
            ctx.AddObjectToAsset("Questionnaire", quest);
            ctx.SetMainObject(quest);
        }
    }

    [System.Serializable]
    class ImportQuest
    {
        public Page[] pages;
    }
}
