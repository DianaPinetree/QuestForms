using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine.UIElements;

namespace QuestForms.Internal
{
    [CustomEditor(typeof(QF_QuestionnaireImporter))]
    public class QF_ImporterEditor : ScriptedImporterEditor
    {
        QF_QuestionnaireImporter importer;
        SerializedProperty questionnaire;
        QF_Questionnaire questSO;
        Vector2 scroll;

        Editor questionnaireEditor;
        bool[] foldoutStatus;

        Texture2D headerBackgroundTex;

        public override void OnEnable()
        {
            base.OnEnable();

            importer = target as QF_QuestionnaireImporter;
            if (importer.quest == null) importer.quest = ScriptableObject.CreateInstance<QF_Questionnaire>();
            questionnaire = serializedObject.FindProperty("quest");
            questSO = questionnaire.objectReferenceValue as QF_Questionnaire;
            Color c = new Color(74f / 255f, 74f / 255f, 74f/255f);
            headerBackgroundTex = MakeTex(Screen.width, 1, c);
        }

        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (questSO != null)
            {
                // Draw page title
                GUIStyle title = new GUIStyle(EditorStyles.boldLabel);
                title.alignment = TextAnchor.MiddleCenter;
                title.fontSize += 8;
                GUILayout.Label(questSO.name, title);

                // Draw Pages foldout
                DrawPagesFoldout();
            }
            ApplyRevertGUI();
        }

        private void DrawPagesFoldout()
        {
            if (foldoutStatus == null)
            {
                foldoutStatus = new bool[questSO.pages.Length];
            }

            for (int p = 0; p < questSO.pages.Length; p++)
            {
                Page page = questSO.pages[p];
                foldoutStatus[p] = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutStatus[p], $"Page {p + 1}: {page.ID}");

                // Page
                if (foldoutStatus[p])
                {
                    DrawPage(page);
                }

                // Close this page group
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Separator();
                // Update SO with modifications made
                questSO.pages[p] = page;
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }

        private void DrawPage(Page page)
        {
            GUIStyle titleLabel = new GUIStyle(EditorStyles.boldLabel);
            titleLabel.fontSize += 3;
            titleLabel.normal.background = headerBackgroundTex;
            // Start page content
            GUILayout.BeginVertical("Page Content", "window");

            GUILayout.Label("Header", titleLabel);
            EditorGUI.indentLevel++;
            GUILayout.Label(page.header);
            EditorGUI.indentLevel--;

            GUILayout.Label("Instructions", titleLabel);
            // Indent forward
            EditorGUI.indentLevel++;

            // If instructions empty, give a warning
            if (!string.IsNullOrEmpty(page.instructions))
            {
                GUILayout.Label(page.instructions, EditorStyles.wordWrappedLabel);
            }
            else
            {
                // Warning message in place of text
                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = Color.yellow;
                GUILayout.Label("No Instructions text was found", labelStyle);
            }
            EditorGUI.indentLevel--;

            // Draw questions and check if there is a scale being used
            GUILayout.Label("Questions", titleLabel);
            // Draw and if there is a scale
            if (DrawQuestions(page.questions))
            {
                GUILayout.Label("Scale", titleLabel);
                // Draw scale
                DrawScale(page.scale);
            }

            // End Page Content
            GUILayout.EndVertical();
        }

        // Returns if the drawn questions use a Scale or are option based
        private bool DrawQuestions(Question[] questions)
        {
            bool usesScale = false;
            foreach (Question q in questions)
            {
                usesScale |= q.type == QuestionType.Scale;
                DrawQuestion(q);
            }

            return usesScale;
        }

        // Draws a single question
        private void DrawQuestion(Question q)
        {

            if (q.mandatory)
            {
                GUILayout.Space(5);
                GUIStyle box = new GUIStyle();
                box.normal.background = MakeTex(Screen.width, 1, new Color(92f / 255f, 82f / 255f, 74f / 255f));
                box.border = new RectOffset(10, 10, 10, 10);
                GUILayout.BeginVertical(box);
            }
            else
            {
                GUILayout.BeginVertical("box");
            }
            
            GUIStyle questionHeader = new GUIStyle(EditorStyles.boldLabel);
            questionHeader.fontSize += 1;
            GUILayout.Label(q.ID + "  " + (q.mandatory ? "(Mandatory)" : ""), questionHeader);

            BoldBlock("Type: ", q.qType);
            BoldBlock("Question: ", q.question);

            GUILayout.EndVertical();
        }

        private void DrawScale(string[] scale)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < scale.Length; i++)
            {
                GUILayout.Label($"{i + 1}. {scale[i]}");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
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
