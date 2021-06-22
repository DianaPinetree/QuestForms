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
        [SerializeField]private QF_Questionnaire quest;
        private TextAsset questFile;
        public QF_Questionnaire Questionnaire => quest;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            if (questFile == null) 
            {
                questFile = new TextAsset(File.ReadAllText(ctx.assetPath));
            }
            
            if (quest == null)
            {
                quest = CreateAsset<QF_Questionnaire>(ctx.assetPath);
                Debug.Log("Created new Questionnaire SO from imported asset");
            }

            quest.pages = JsonUtility.FromJson<ImportQuest>(questFile.text).pages;
            quest.hideFlags = HideFlags.None;

            quest.CreateImages();

            if (questFile != null)
            {
                Texture2D thumb = (Texture2D)EditorGUIUtility.Load("Icons/QF_Importer Icon.png");
                ctx.AddObjectToAsset("Form", questFile, thumb);
                ctx.SetMainObject(questFile);
            }
        }

        public static T CreateAsset<T>(string path) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(path), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }
    }

    [System.Serializable]
    class ImportQuest
    {
        public Page[] pages;
    }
}
