using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine.UIElements;
using QuestForms;

namespace QuestForms.Internal
{
    [CustomEditor(typeof(QF_Questionnaire))]
    public class QF_ImporterEditor : Editor
    {
        const int IMAGE_PREVIEW_SIZE = 250;
        QF_Questionnaire questSO;
        Vector2 scroll;
        SerializedProperty pagesProperty;
        SerializedProperty imageList;

        bool[] foldoutStatus;

        Texture2D headerBackgroundTex;
        GUIStyle titleLabel;

        public void OnEnable()
        {
            // Get properties
            pagesProperty = serializedObject.FindProperty("pages");
            imageList = serializedObject.FindProperty("images");

            questSO = (QF_Questionnaire)target;
            Color c = new Color(74f / 255f, 74f / 255f, 74f / 255f);
            headerBackgroundTex = MakeTex(Screen.width, 1, c);
        }

        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            titleLabel = new GUIStyle(EditorStyles.boldLabel);
            titleLabel.fontSize += 3;
            titleLabel.normal.background = headerBackgroundTex;

            // Draw page title
            GUIStyle title = new GUIStyle(EditorStyles.boldLabel);
            title.alignment = TextAnchor.MiddleCenter;
            title.fontSize += 8;
            GUILayout.Label(questSO.name, title);

            // Draw Pages foldout
            DrawPagesFoldout();
            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(questSO);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void DrawPagesFoldout()
        {
            if (foldoutStatus == null)
            {
                foldoutStatus = new bool[questSO.pages.Length];
                foldoutStatus[0] = true;
            }

            // Header
            GUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Navigation", titleLabel);
            scroll = GUILayout.BeginScrollView(scroll);
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < foldoutStatus.Length; i++)
            {
                if (GUILayout.Button($"Pg. {i + 1}"))
                {
                    // Expand selected
                    foldoutStatus[i] = true;
                    // Close the rest
                    for (int k = 0; k < foldoutStatus.Length; k++)
                    {
                        if (i == k) continue;
                        foldoutStatus[k] = false;
                    }
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(5);

            for (int p = 0; p < questSO.pages.Length; p++)
            {
                Page page = questSO.pages[p];

                // Page
                if (foldoutStatus[p])
                {

                    // Start page content
                    GUILayout.BeginVertical($"Page {p + 1} -- {page.ID}", "window");
                    DrawPage(p);
                    // End Page Content
                    GUILayout.EndVertical();
                }
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

        private void DrawImagePreview(SerializedProperty pair)
        {
            GUILayout.Label("Image", titleLabel);
            SerializedProperty anchor = pair.FindPropertyRelative("position");
            SerializedProperty image = pair.FindPropertyRelative("image");

            anchor.enumValueIndex = (int)(ImageAnchor)EditorGUILayout.EnumPopup((ImageAnchor)anchor.enumValueIndex);
            image.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField(image.objectReferenceValue, typeof(Sprite), false);


            if (image.objectReferenceValue != null)
            {
                Texture tex = (image.objectReferenceValue as Sprite).texture;
                Rect imageRect = (image.objectReferenceValue as Sprite).rect;
                if ((ImageAnchor)anchor.enumValueIndex == ImageAnchor.Right)
                {
                    GUILayout.FlexibleSpace();
                }

                float ratio = imageRect.height / imageRect.width;
                Rect r = EditorGUILayout.GetControlRect(GUILayout.MaxHeight(IMAGE_PREVIEW_SIZE), GUILayout.MaxWidth(IMAGE_PREVIEW_SIZE / ratio));
                EditorGUI.DrawPreviewTexture(r, tex);
            }
        }

        private void DrawPage(int pageIndex)
        {
            Page page = questSO.pages[pageIndex];
            SerializedProperty pageProperty = pagesProperty.GetArrayElementAtIndex(pageIndex);

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

            GUILayout.Label("Page Format", titleLabel);
            PageFormat(pageProperty);

            if (questSO.ContainsImage(page.ID))
            {
                int index = questSO.ImagePair(page.ID);
                
                DrawImagePreview(imageList.GetArrayElementAtIndex(index));
            }

            // Draw questions and check if there is a scale being used
            GUILayout.Label("Questions", titleLabel);
            // Draw and if there is a scale
            if (DrawQuestions(page.questions))
            {
                GUILayout.Label("Scale", titleLabel);
                // Draw scale
                DrawScale(page.scale);
            }
        }

        private void PageFormat(SerializedProperty page)
        {
            SerializedProperty scrollformat = page.FindPropertyRelative("scrollQuestions");
            // Scroll questions bool

            scrollformat.enumValueIndex = (int)(ScrollType)EditorGUILayout.EnumPopup((ScrollType)scrollformat.enumValueIndex);

            if ((ScrollType)(scrollformat.enumValueIndex) == ScrollType.SplitToPage)
            {
                var questions = page.FindPropertyRelative("questions");
                GUILayout.Label($"Split into: {QF_Rules.QuestionsPerPage / questions.arraySize} page", EditorStyles.boldLabel);
            }
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

            if (questSO.ContainsImage(q.ID))
            {
                int index = questSO.ImagePair(q.ID);
                DrawImagePreview(imageList.GetArrayElementAtIndex(index));
            }

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
