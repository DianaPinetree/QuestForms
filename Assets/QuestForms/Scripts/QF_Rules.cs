using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;


/*
NOTES:
- Gravar apenas para json
- Shuffle de questões por página
*/

namespace QuestForms
{
    /// <summary>
    /// Contains all data for button text, colors, other rules
    /// </summary>
    public class QF_Rules
    {
        private static int questionsPerPage = 6;
        private static int optionsSpacing = 5;
        private static int questionSpacing = 25;
        private static int questionHeaderFont = 20;
        private static int questionFont = 18;
        private static int maxTextFieldLines = 8;
        private static TMP_FontAsset textFont;
        private static TMP_FontAsset headerFont;

        public static TMP_FontAsset TextFont { get => textFont;}
        public static TMP_FontAsset HeaderFont { get => headerFont;}
        public static int QuestionsPerPage { get => questionsPerPage;}
        public static int QuestionSpacing => questionSpacing;
        public static GameObject Seperator => Resources.Load<GameObject>("QF_Seperator");
        public static int MaxTextFieldLines => maxTextFieldLines;

        public static int HeaderFontSize { get => questionHeaderFont; }
        public static int QuestionFontSize { get => questionFont; }
        public static int OptionsSpacing { get => optionsSpacing;}

        [MenuItem("Tools/QuestForms/Create Questionnaire Canvas")]
        public static void CreateQuestionnaireCanvas()
        {
            GameObject canvas = new GameObject("Questionnaire");

            var c = canvas.AddComponent<Canvas>();
            var scaler = canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            var form = canvas.AddComponent<QF_QuestForm>();

            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
            c.targetDisplay = 0;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        }

        [MenuItem("Assets/Create/Questionnaire")]
        public static void CreateQuestionnaire()
        {
            QF_Questionnaire quest = Selection.activeObject as QF_Questionnaire;
            GameObject canvas = new GameObject($"Questionnaire {quest.name}");

            var c = canvas.AddComponent<Canvas>();
            var scaler = canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            var form = canvas.AddComponent<QF_QuestForm>();

            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
            c.targetDisplay = 0;

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
        
            form.SetQuestionnaire(quest, true);
        }

        [MenuItem("Assets/Create/Questionnaire", true)]
        public static bool CreateQuestionnaireValidation()
        {
            return Selection.activeObject is QF_Questionnaire;
        }
    }
}
