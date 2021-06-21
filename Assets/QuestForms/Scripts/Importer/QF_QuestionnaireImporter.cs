using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using QuestForms;

namespace QuestForms.Internal
{
    [ScriptedImporter(4, "quest")]
    public class QF_QuestionnaireImporter : ScriptedImporter
    {
        public QF_Questionnaire quest;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (quest == null)
            {
                quest = ScriptableObject.CreateInstance<QF_Questionnaire>();
                Debug.Log("Created new Questionnaire SO from imported asset");

                quest.pages = JsonUtility.FromJson<ImportQuest>(File.ReadAllText(ctx.assetPath)).pages;
                quest.hideFlags = HideFlags.None;
                ctx.AddObjectToAsset("Questionnaire", quest);
                ctx.SetMainObject(quest);

                EditorUtility.SetDirty(quest);
            }
        }
    }

    [System.Serializable]
    class ImportQuest
    {
        public Page[] pages;
    }
}
