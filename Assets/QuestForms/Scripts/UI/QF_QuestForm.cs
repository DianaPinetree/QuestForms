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
        /// Currently loaded questionnaire
        /// </summary>
        [SerializeField] private QF_Questionnaire questionnaire;
        /// <summary>
        /// Page list of gameobjects pages
        /// </summary>
        [SerializeField] private List<QF_Page> pagesInstance;
        [SerializeField, ExporterList] private string exportType;
        [SerializeField] private string fileName = "MyQuestionnaireData";
        [SerializeField] private string savePath;
        public List<IAnswerElement> questions = new List<IAnswerElement>();

        public QF_Questionnaire QuestSource => questionnaire;
        public List<QF_Page> Pages => pagesInstance;

        /// <summary>
        /// Current page displaying - playmode only
        /// </summary>
        private int pageDisplayIndex;

        private void Awake()
        {
            pageDisplayIndex = 0;
        }

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
                StringBuilder message = new StringBuilder("Questões Inválidas. Completa as seguintes questões para prosseguir: ");

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

        public void SetPage(int page, bool state)
        {
            SetPage(pagesInstance[page], state);
        }

        public void OnQuestionnaireEnd()
        {
            onQuestionnaireEnd?.Invoke();
        }

        public void ExportAnswers()
        {
            Type t = System.Type.GetType(exportType);
            IQuestionnaireExporter exporter =  (IQuestionnaireExporter)Activator.CreateInstance(t);

            string data = exporter.FormatData(questions);

            if (!string.IsNullOrEmpty(savePath))
            {
                SaveDataToFile(data, fileName, exporter.Extension);
            }
            onExport?.Invoke(data);
        }

        private void SaveDataToFile(string data, string fileName, string extension)
        {
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

        public static event System.Action onQuestionnaireEnd;
        public static event System.Action<string> onExport;

        // Dont mess with the UI generation, might break if you don't know what you're doing
        #region Editor UI Generation
#if UNITY_EDITOR
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
