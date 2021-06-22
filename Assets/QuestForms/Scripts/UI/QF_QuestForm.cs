using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        /// Page list of gameobejct pages
        /// </summary>
        [SerializeField] private List<QF_Page> pagesInstance;

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

        
        public void NextPage() 
        {
            Debug.Log("Next Questionnaire Page");

        }

        public void PreviousPage() 
        {
            Debug.Log("Previous Questionnaire Page");

        }

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

            // Create questions
            CreateQuestions();
        }

        public void CleanUp()
        {
            if (transform.childCount <= 0) return;

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public void SetupStructure()
        {
            GameObject pagePrefab = Resources.Load<GameObject>("QF_PagePrefab");
            
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
        }

        public void CreateQuestions()
        {

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
