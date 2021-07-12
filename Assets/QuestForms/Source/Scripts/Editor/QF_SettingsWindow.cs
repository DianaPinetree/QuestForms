using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using QuestForms;

namespace QuestForms.Internal
{
    public class QF_SettingsWindow : EditorWindow
    {
        private static QF_Rules settings;
        private Texture2D headerBackgroundTex;
        private Vector2 scroll;
        private string exportLanguageFileName = "language";

        /// <summary>
        /// Initializes the editor window
        /// </summary>
        [MenuItem("Tools/QuestForms/Settings")]
        public static void Init()
        {
            QF_SettingsWindow window = (QF_SettingsWindow)EditorWindow.GetWindow<QF_SettingsWindow>("Quest Form Settings");
            
            // Get settings
            settings = QF_Rules.Instance;
            window.minSize = new Vector2(400f, 100f);
            window.Show();
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            hasUnsavedChanges = false;
            
            // Create language table again with any given changes
            settings.language.CreateLanguageTable();
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                settings = QF_Rules.Instance;
            }
            if (headerBackgroundTex == null)
            {
                Color c = new Color(74f / 255f, 74f / 255f, 74f / 255f);
                headerBackgroundTex = QF_QuestionnaireEditor.MakeTex(Screen.width, 1, c);
            }

            GUIStyle subtitle = new GUIStyle(EditorStyles.boldLabel);
            subtitle.fontSize += 3;
            subtitle.normal.background = headerBackgroundTex;

            GUIStyle title = new GUIStyle(EditorStyles.boldLabel);
            title.alignment = TextAnchor.MiddleCenter;
            title.fontSize += 5;

            GUILayout.Label("Quest Forms Settings", title);

            scroll = EditorGUILayout.BeginScrollView(scroll);

            // Draw each setting
            GUILayout.Label("Text Settings", subtitle);
            TextSettings();

            GUILayout.Label("Color Settings", subtitle);
            ColorSettings();

            GUILayout.Label("Language Settings", subtitle);
            LanguageSettings();

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            OperationButtons();
            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                hasUnsavedChanges = true;
            }

            if (GUILayout.Button("Save", GUILayout.Height(40f)))
            {
                SaveChanges();
            }

        }

        public void OperationButtons()
        {
            if (GUILayout.Button("Reset", GUILayout.Height(40f)))
            {
                settings.Reset();
                SaveChanges();
            }
        }

        public void TextSettings()
        {
            settings.questionHeaderFont = EditorGUILayout.IntField(
                "Header Font Size", settings.questionHeaderFont);

            settings.questionFont = EditorGUILayout.IntField(
                "Body Font Size", settings.questionFont);
            
            settings.maxTextFieldLines = EditorGUILayout.IntField(
                "Input Field Max Lines", settings.maxTextFieldLines);

            if (settings.textFont == null)
            {
                settings.textFont = TMPro.TMP_Settings.defaultFontAsset;
            }

            settings.textFont = (TMPro.TMP_FontAsset)EditorGUILayout.ObjectField(
                "Body Font Asset", settings.textFont, typeof(TMPro.TMP_FontAsset), false);

            if (settings.headerFont == null)
            {
                settings.headerFont = TMPro.TMP_Settings.defaultFontAsset;
            }
            
            settings.headerFont = (TMPro.TMP_FontAsset)EditorGUILayout.ObjectField(
                "Body Font Asset", settings.headerFont, typeof(TMPro.TMP_FontAsset), false);


            settings.questionSpacing = EditorGUILayout.IntField(
                "Questions Spacing", settings.questionSpacing);
        
            settings.optionsSpacing = EditorGUILayout.IntField(
                "Options Spacing", settings.optionsSpacing);
        }

        public void ColorSettings()
        {
            settings.primaryColor = EditorGUILayout.ColorField("Primary Color", settings.primaryColor);
            settings.secondaryColor = EditorGUILayout.ColorField("Secondary Color", settings.secondaryColor);
            settings.accentColor = EditorGUILayout.ColorField("Accent Color", settings.accentColor);
            settings.invalidColor = EditorGUILayout.ColorField("Invalid Color", settings.invalidColor);
            settings.textColor = EditorGUILayout.ColorField("Text Color", settings.textColor);
            settings.backgroundColor = EditorGUILayout.ColorField("Background Color", settings.backgroundColor);
        }

        public void LanguageSettings()
        {
            GUILayout.Label("General", EditorStyles.boldLabel);
            settings.language.nextButtonText = EditorGUILayout.TextField("Next Button Text", settings.language.nextButtonText);
            settings.language.backButtonText = EditorGUILayout.TextField("Back Button Text", settings.language.backButtonText);
            settings.language.clearButtonText = EditorGUILayout.TextField("Clear Button Text", settings.language.clearButtonText);
            settings.language.finishButtonText = EditorGUILayout.TextField("Finish Button Text", settings.language.finishButtonText);
            
            settings.language.instructionsText = EditorGUILayout.TextField("Instructions Text", settings.language.instructionsText);
            settings.language.incompleteText = EditorGUILayout.TextField("Incomplete page message", settings.language.incompleteText);

            GUILayout.Space(10);
            GUILayout.Label("Confirmation Page", EditorStyles.boldLabel);
            settings.language.endpageHeaderText = EditorGUILayout.TextField("Page Header", settings.language.endpageHeaderText);
            GUILayout.Label("Message Text");
            settings.language.endMessageText = EditorGUILayout.TextArea(settings.language.endMessageText);
            GUILayout.Label("Data Authorization Text");
            settings.language.dataAuthText = EditorGUILayout.TextArea(settings.language.dataAuthText);
            settings.language.confirmText = EditorGUILayout.TextField("Confirm text", settings.language.confirmText);
            settings.language.cancelText = EditorGUILayout.TextField("Cancel text", settings.language.cancelText);

            if (GUILayout.Button("Import Language File", GUILayout.Height(25f)))
            {
                string path = EditorUtility.OpenFilePanel("Import Language Json File", Application.dataPath, "json");
                if (!string.IsNullOrEmpty(path))
                {
                    ImportLanguageFile(path);
                }
            }

            EditorGUILayout.BeginHorizontal();
            exportLanguageFileName = EditorGUILayout.TextField("File Name", exportLanguageFileName);
            
            bool validName = ValidFileName(exportLanguageFileName);
            if (GUILayout.Button("Export Language",  GUILayout.Height(25f)))
            {
                if (validName)
                {
                    string path = EditorUtility.OpenFolderPanel("Export Language", Application.dataPath, "");
                    path += "/" + exportLanguageFileName + ".json";
                    if (!string.IsNullOrEmpty(path))
                    {
                        ExportLanguageFile(path);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (!validName)
            {
                EditorGUILayout.HelpBox("File name cannot be empty to export", MessageType.Warning);
            }
        }


        private void ImportLanguageFile(string path)
        {
            string json;
            using (StreamReader reader = new StreamReader(path))
            {
                json = reader.ReadToEnd();
            }

            LanguageSettings importedLanguage = JsonUtility.FromJson<LanguageSettings>(json);
            settings.language = importedLanguage;
        }

        private bool ValidFileName(string fileName)
        {
            bool valid = !string.IsNullOrEmpty(fileName);
            if(!valid)
            {
                
            }

            return valid;
        }

        private void ExportLanguageFile(string path)
        {
            string json = JsonUtility.ToJson(settings.language, true);

            using(StreamWriter writer = File.CreateText(path))
            {
                writer.Write(json);
                writer.Close();
            }
        }
    }
}
