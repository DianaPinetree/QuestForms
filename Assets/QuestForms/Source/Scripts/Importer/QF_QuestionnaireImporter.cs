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
                quest = Util.CreateAsset<QF_Questionnaire>(ctx.assetPath, select: true);
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
    }

    [System.Serializable]
    class ImportQuest
    {
        public Page[] pages;
    }
}

namespace QuestForms
{
    public static class Util
    {
        /// <summary>
        /// Creates a scriptable object asset in the given path
        /// </summary>
        /// <param name="path">Path to create the scriptable object at, starting at the assets folder. Ex.: (Assets/Resources)</param>
        /// <param name="assetName">File name of the asset, if no name is given it will create the asset with the blueprint "New AssetTypeName"</param>
        /// <param name="select">Should select the asset in editor after creating</param>
        /// <typeparam name="T">Scriptable Object type of the asset to be created</typeparam>
        /// <returns> The created asset instance</returns>
        public static T CreateAsset<T>(string path, string assetName = "", bool select = false) where T : ScriptableObject
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

            if (assetName == string.Empty)
            {
                assetName = "New " + typeof(T).Name;
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + assetName + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (select)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }

            return asset;
        }
    }
}