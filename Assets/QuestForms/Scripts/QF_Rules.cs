using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    /// <summary>
    /// Contains all data for button text, colors, other rules
    /// </summary>
    public class QF_Rules
    {
        private static int questionsPerPage = 6;
        private static TMP_FontAsset textFont;
        private static TMP_FontAsset headerFont;

        public static TMP_FontAsset TextFont { get => textFont;}
        public static TMP_FontAsset HeaderFont { get => headerFont;}
        public static int QuestionsPerPage { get => questionsPerPage;}
        public static GameObject Seperator => Resources.Load<GameObject>("QF_Seperator");


        [MenuItem("Tools/QuestForms/Create Questionnaire")]
        public static void AddQuestForm()
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
        
            if (Selection.activeObject is QF_Questionnaire) 
            {
                form.SetQuestionnaire(Selection.activeObject as QF_Questionnaire, true);
            }
        }
    }
}
