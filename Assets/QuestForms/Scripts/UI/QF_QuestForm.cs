using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;

namespace QuestForms
{
    /// <summary>
    /// Acts as the manager
    /// </summary>
    public class QF_QuestForm : MonoBehaviour
    {
        /// <summary>
        /// Array of questionnaires to be loaded, this array is combined and put into questionnaire variable
        /// </summary>
        [SerializeField] private QF_Questionnaire[] form;
        /// <summary>
        /// Currently loaded questionnaire
        /// </summary>
        [SerializeField] private QF_Questionnaire questionnaire;
        /// <summary>
        /// Page list of gameobjects pages
        /// </summary>
        [SerializeField] private List<QF_Page> pagesInstance;

        // Save data variables
        /// <summary>
        /// If the questionnaire should be saved locally
        /// </summary>
        [SerializeField] private bool saveDataLocally;
        /// <summary>
        /// Save path should be application persistent data path
        /// </summary>
        
        [SerializeField, Tooltip("Use this when you don't know which computer this will run on")] 
        private bool usePersistentDataPath;
        /// <summary>
        /// Export type name of the questionnaire answers
        /// </summary>
        [SerializeField, ExporterList] private string exportType;
        /// <summary>
        /// Export file name
        /// </summary>
        [SerializeField] private string fileName = "MyQuestionnaireData";
        /// <summary>
        /// Full save path
        /// </summary>
        [SerializeField] private string savePath;

        /// <summary>
        /// Questions in the questionnaire, in the generated order
        /// </summary>
        public List<IAnswerElement> questions = new List<IAnswerElement>();

        /// <summary>
        /// Retrieve the complete currently loaded questionnaire;
        /// </summary>
        public QF_Questionnaire QuestSource => questionnaire;
        /// <summary>
        /// Gameobject Page list of the created questionnaire 
        /// </summary>
        public List<QF_Page> Pages => pagesInstance;

        /// <summary>
        /// Current page displaying - playmode only
        /// </summary>
        private int pageDisplayIndex;

        private void Awake()
        {
            pageDisplayIndex = 0;
        }

        /// <summary>
        /// Unity start, retrieves questions in the generated questionnaire and initializes pages
        /// If a page is meant to be randomize, randomize orders
        /// </summary>
        private void Start()
        {
            questions = new List<IAnswerElement>(GetComponentsInChildren<IAnswerElement>(true));
            Debug.Log(questions.Count);

            SetPage(pageDisplayIndex, true);
            foreach (QF_Page page in pagesInstance)
            {
                if (page.RandomizeContent)
                {
                    page.ShuffleContent();
                }
            }
        }
        
        /// <summary>
        /// Check validity of currently displaying page
        /// </summary>
        /// <returns> Returns the Validity of the page</returns>
        public bool CheckForInvalidPage()
        {
            QF_QuestionGroup[] groups = pagesInstance[pageDisplayIndex].GetComponentsInChildren<QF_QuestionGroup>();

            List<int> invalid = new List<int>();
            for (int i = 0; i < groups.Length; i++)
            {
                if (!groups[i].Valid())
                {
                    invalid.Add(i + 1);
                }
            }

            if (invalid.Count > 0)
            {
                StringBuilder message = new StringBuilder(QF_Rules.Instance.language["incomplete message"] + ": ");

                for (int i = 0; i < invalid.Count; i++)
                {
                    if (i == invalid.Count - 1)
                    {
                        message.Append($" {invalid[i]}");
                    }
                    else
                    {
                        message.Append($" {invalid[i]},");
                    }

                }
                pagesInstance[pageDisplayIndex].PageMessage(message.ToString());
                return false;
            }
            else
            {
                pagesInstance[pageDisplayIndex].PageMessage("");
            }

            return true;
        }

        /// <summary>
        /// Moves to the next Questionnaire page, called in ui button
        /// </summary>
        public void NextPage()
        {
            if (!CheckForInvalidPage())
            {
                return;
            }

            if ((pageDisplayIndex + 1) == Pages.Count)
            {
                SetPage(pageDisplayIndex, false);

                transform.GetChild(transform.childCount - 1).gameObject.SetActive(true);
            }
            else
            {
                SetPage(pageDisplayIndex, false);
                pageDisplayIndex = Mathf.Clamp((pageDisplayIndex + 1), 0, Pages.Count - 1);
                SetPage(pageDisplayIndex, true);
            }
        }

        /// <summary>
        /// Moves to the previous Questionnaire page, called in UI button
        /// </summary>
        public void PreviousPage()
        {
            SetPage(pageDisplayIndex, false);
            pageDisplayIndex = Mathf.Clamp((pageDisplayIndex - 1), 0, Pages.Count - 1);
            SetPage(pageDisplayIndex, true);
        }

        /// <summary>
        /// Selects last page
        /// </summary>
        public void LastPage()
        {
            SetPage(pageDisplayIndex, false);
            pageDisplayIndex = Pages.Count - 1;
            SetPage(pageDisplayIndex, true);
        }

        /// <summary>
        /// Sets a given page to be active or innactive
        /// </summary>
        /// <param name="page"></param>
        /// <param name="state"></param>
        public void SetPage(QF_Page page, bool state)
        {
            CanvasGroup c = page.GetComponent<CanvasGroup>();
            c.interactable = state;
            c.blocksRaycasts = state;

            page.gameObject.SetActive(state);
        }

        /// <summary>
        /// Sets page state by index of pageInstance list
        /// </summary>
        /// <param name="page">index of the page to change state</param>
        /// <param name="state"> state to change to</param>
        public void SetPage(int page, bool state)
        {
            SetPage(pagesInstance[page], state);
        
        }

        /// <summary>
        /// Calls an event when the questionnaire ends
        /// </summary>
        public void OnQuestionnaireEnd()
        {
            onQuestionnaireEnd?.Invoke();
        }

        /// <summary>
        /// Exports the answers in the questionnaire, if it is meant to save to a file, export it
        /// </summary>
        public void ExportAnswers()
        {
            Type t = System.Type.GetType(exportType);
            IQuestionnaireExporter exporter =  (IQuestionnaireExporter)Activator.CreateInstance(t);

            string data = exporter.FormatData(questions);

            if (saveDataLocally && !string.IsNullOrEmpty(savePath))
            {
                SaveDataToFile(data, fileName, exporter.Extension);
            }

            onExport?.Invoke(data);
        }

        /// <summary>
        /// Creates a file and dumps the data variable into it, saving it into the savePath path
        /// </summary>
        /// <param name="data">formatted data string</param>
        /// <param name="fileName">name of the file to create</param>
        /// <param name="extension">extension of the file</param>
        private void SaveDataToFile(string data, string fileName, string extension)
        {
            if (usePersistentDataPath)
            {
                savePath = Application.persistentDataPath;
            }

            string filePath = savePath + "/" + fileName + extension;

            int count = 0;
            while(File.Exists(filePath))
            {
                count++;
                filePath = savePath + "/" + fileName + count.ToString() + extension;
            }

            using (StreamWriter writer = File.CreateText(filePath))
            {
                writer.WriteLine(data);
            }
        }

        /// <summary>
        /// Called when the questionnaire ends, use this to change scene, close the application etc..
        /// </summary>
        public static event System.Action onQuestionnaireEnd;
        /// <summary>
        /// Export callback, called when a questionnaire is exported, with the exported data as the parameter
        /// </summary>
        public static event System.Action<string> onExport;

        // Dont mess with the UI generation, might break if you don't know what you're doing
        #region Editor UI Generation
#if UNITY_EDITOR

        private void OnValidate() 
        {
            List<Page> pages = new List<Page>();
            List<QF_ImagePair> images = new List<QF_ImagePair>();

            for (int i = 0; i < form.Length; i++)
            {
                if (form[i] == null) continue;
                pages.AddRange(form[i].pages);
                images.AddRange(form[i].images);
            }

            questionnaire = ScriptableObject.CreateInstance<QF_Questionnaire>();

            questionnaire.images = images;
            questionnaire.pages = pages.ToArray();
        }

        public void SetQuestionnaire(QF_Questionnaire q, bool load)
        {
            if (q == null) return;
            questionnaire = q;

            if (load)
            {
                LoadQuestionnaire();
            }
        }

        public void LoadQuestionnaire()
        {
            // Cleanup Existing
            CleanUp();

            // Setup page structure
            SetupStructure();

            QF_TextFont[] textElements = GetComponentsInChildren<QF_TextFont>(true);
            QF_Localiser[] localizedTextElements = GetComponentsInChildren<QF_Localiser>(true);
            QF_Color[] coloredElements = GetComponentsInChildren<QF_Color>(true);

            foreach(QF_TextFont textFont in textElements)
            {
                textFont.OnValidate();
            }

            foreach(QF_Localiser localizedText in localizedTextElements)
            {
                localizedText.OnValidate();
            }

            foreach(QF_Color colorImg in coloredElements)
            {
                colorImg.OnValidate();
            }
        }

        public void CleanUp()
        {
            if (transform.childCount <= 0) return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            questions.Clear();
        }

        public void SetupStructure()
        {
            GameObject pagePrefab = Resources.Load<GameObject>("QF_PagePrefab");
            GameObject endConfirmation = Resources.Load<GameObject>("QF_Confirmation");

            if (pagesInstance == null) pagesInstance = new List<QF_Page>();

            pagesInstance.Clear();

            for (int i = 0; i < questionnaire.pages.Length; i++)
            {
                Page page = questionnaire.pages[i];
                GameObject inst = Instantiate(pagePrefab, transform);

                inst.name = "QF_Page -- " + page.ID;
                var p = inst.AddComponent<QF_Page>();
                inst.AddComponent<CanvasGroup>();

                p.Init(i, this);
                pagesInstance.Add(p);

                if (i != 0) SetPage(p, false);
            }

            if (endConfirmation)
            {
                endConfirmation = Instantiate(endConfirmation, transform);
                endConfirmation.name = "QF_Confirmation";

                var cfPage = endConfirmation.AddComponent<QF_ConfirmationPage>();
                cfPage.manager = this;
            }
        }

        public void PrintData()
        {
            foreach (var page in questionnaire.pages)
            {
                Debug.Log($"Page {page.ID}");

                foreach (var question in page.questions)
                {

                }
            }

            Debug.Log("[QuestForms]: Created new Questionnaire UI!");
        }
#endif
        #endregion
    }
}
