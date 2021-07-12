using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System;

namespace QuestForms
{
    /// <summary>
    /// Contains all data for button text, colors, other rules
    /// </summary>
    public class QF_Rules : ScriptableObject
    {
        #region Default Values

        private const int QUESTIONS_PER_PAGE = 6;
        private const int OPTIONS_SPACING = 5;
        private const int QUESTIONS_SPACING = 25;
        private const int QUESTIONS_HEADER_FONTSIZE = 20;
        private const int QUESTIONS_BODY_FONTSIZE = 18;
        private const int MAX_TEXTFIELD_LINES = 8;

        /*
            #a5d6a7 - Primary
            #75a478 - P Dark (Headers for example)
            #d7ffd9 - P Light (Sub Headers, third component)
            #FFD7D7 - Unavailable
        */
        private static readonly Color PRIMARY_COLOR = new Color(0.6470588f, 0.8392157f, 0.654902f, 1f);
        private static readonly Color SECONDARY_COLOR = new Color(0.4588235f, 0.6431373f, 0.4705882f, 1f);
        private static readonly Color ACCENT_COLOR = new Color(0.84f, 1f, 0.85f, 1f);
        private static readonly Color INVALID_COLOR = new Color(1f, 0.8431373f, 0.8431373f, 1f);
        private static readonly Color TEXT_COLOR = new Color(0.1f, 0.1f, 0.1f, 1f);
        private static readonly Color BACKGROUND_COLOR = new Color(.95f, .95f, .95f, 1f);

        private static readonly string BACK_BUTTON_TEXT = "Anterior";
        private static readonly string NEXT_BUTTON_TEXT = "Continuar";
        private static readonly string CLEAR_BUTTON_TEXT = "Limpar";
        private static readonly string FINISH_BUTTON_TEXT = "Terminar";

        private static readonly string ENDPAGE_HEADER_TEXT = "Terminar e Enviar";
        private static readonly string END_MESSAGE_TEXT = "Obrigado pelo tempo despendido";
        private static readonly string DATA_AUTH_TEXT = "Autoriza a gravação anónima dos dados anteriormente respondidos?";
        private static readonly string CONFIRM_TEXT = "Sim";
        private static readonly string CANCEL_TEXT = "Não";

        private static readonly string INCOMPLETE_QUESTIONS = "Questões Inválidas. Completa as seguintes questões para prosseguir: ";

        
        #endregion
        /// <summary>
        /// Lazy instance of the QF rules
        /// </summary>
        /// <typeparam name="QF_Rules"></typeparam>
        /// <returns></returns>
        private static Lazy<QF_Rules> instance = new Lazy<QF_Rules>(() =>
        {
            QF_Rules loaded = Resources.Load<QF_Rules>("QuestForms Settings");

            if (loaded == null)
            {
                loaded = Util.CreateAsset<QF_Rules>("Assets/QuestForms/Resources", "QuestForms Settings");
            }

            return loaded;
        });

        // Text settings
        [HideInInspector] public int questionsPerPage = QUESTIONS_PER_PAGE;
        [HideInInspector] public int optionsSpacing = OPTIONS_SPACING;
        [HideInInspector] public int questionSpacing = QUESTIONS_SPACING;
        [HideInInspector] public int questionHeaderFont = QUESTIONS_HEADER_FONTSIZE;
        [HideInInspector] public int questionFont = QUESTIONS_BODY_FONTSIZE;
        [HideInInspector] public int maxTextFieldLines = MAX_TEXTFIELD_LINES;
        [HideInInspector] public TMP_FontAsset textFont;
        [HideInInspector] public TMP_FontAsset headerFont;

        // Color settings
        [HideInInspector] public Color primaryColor = PRIMARY_COLOR;
        [HideInInspector] public Color secondaryColor = SECONDARY_COLOR;
        [HideInInspector] public Color accentColor = ACCENT_COLOR;
        [HideInInspector] public Color invalidColor = INVALID_COLOR;
        [HideInInspector] public Color textColor = TEXT_COLOR;
        [HideInInspector] public Color backgroundColor = TEXT_COLOR;

        // Language Settings
        [HideInInspector] public LanguageSettings language;

        public static QF_Rules Instance => instance.Value;
        public static TMP_FontAsset TextFont { get => Instance.textFont; }
        public static TMP_FontAsset HeaderFont { get => Instance.headerFont; }
        public static int QuestionsPerPage { get => Instance.questionsPerPage; }
        public static int QuestionSpacing => Instance.questionSpacing;
        public static GameObject Seperator => Resources.Load<GameObject>("QF_Seperator");
        public static int MaxTextFieldLines => Instance.maxTextFieldLines;

        public static int HeaderFontSize { get => Instance.questionHeaderFont; }
        public static int QuestionFontSize { get => Instance.questionFont; }
        public static int OptionsSpacing { get => Instance.optionsSpacing; }

        /// <summary>
        /// Resets all setting values
        /// </summary>
        public void Reset()
        {
            questionsPerPage = QUESTIONS_PER_PAGE;
            optionsSpacing = OPTIONS_SPACING;
            questionSpacing = QUESTIONS_SPACING;
            questionHeaderFont = QUESTIONS_HEADER_FONTSIZE;
            questionFont = QUESTIONS_BODY_FONTSIZE;
            maxTextFieldLines = MAX_TEXTFIELD_LINES;

            textFont = TMP_Settings.defaultFontAsset;
            headerFont = TMP_Settings.defaultFontAsset;

            primaryColor = PRIMARY_COLOR;
            secondaryColor = SECONDARY_COLOR;
            accentColor = ACCENT_COLOR;
            invalidColor = INVALID_COLOR;
            textColor = TEXT_COLOR;
            backgroundColor = BACKGROUND_COLOR;

            language.backButtonText = BACK_BUTTON_TEXT;
            language.nextButtonText = NEXT_BUTTON_TEXT;
            language.clearButtonText = CLEAR_BUTTON_TEXT;
            language.finishButtonText = FINISH_BUTTON_TEXT;
            language.endpageHeaderText = ENDPAGE_HEADER_TEXT;
            language.endMessageText = END_MESSAGE_TEXT;
            language.dataAuthText = DATA_AUTH_TEXT;
            language.confirmText = CONFIRM_TEXT;
            language.cancelText = CANCEL_TEXT;
            language.incompleteText = INCOMPLETE_QUESTIONS;
        }

        #region Editor Menu Items
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
        #endregion
    }

    /// <summary>
    /// Data structure that holds the current language of the questionnaire
    /// </summary>
    [System.Serializable]
    public class LanguageSettings
    {
        public string backButtonText;
        public string nextButtonText;
        public string clearButtonText;
        public string finishButtonText;
        public string instructionsText;
        public string endpageHeaderText;
        public string endMessageText;
        public string dataAuthText;
        public string confirmText;
        public string cancelText;
        public string incompleteText;
        private IDictionary<string, string> languageTable;
        public IDictionary<string, string> LanguageTable 
        {
            get 
            {
                if (languageTable == null) CreateLanguageTable();

                return languageTable;
            }
        }

        public string this[string key]
        {
            get
            {
                if (languageTable == null)
                {
                    CreateLanguageTable();
                }

                if (key == null || !languageTable.ContainsKey(key))
                {
                    return "KEY_NOT_FOUND";
                }

                return languageTable[key];
            }
        }

        public void CreateLanguageTable()
        {
            languageTable = new Dictionary<string, string>();
            languageTable.Add("back", backButtonText);
            languageTable.Add("next", nextButtonText);
            languageTable.Add("clear", clearButtonText);
            languageTable.Add("instructions", instructionsText);
            languageTable.Add("finish", finishButtonText);
            languageTable.Add("end page", endpageHeaderText);
            languageTable.Add("end message", endMessageText);
            languageTable.Add("data authorization", dataAuthText);
            languageTable.Add("confirm", confirmText);
            languageTable.Add("cancel", cancelText);
            languageTable.Add("incomplete message", incompleteText);
        }
    }
}
