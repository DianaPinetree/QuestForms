using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace QuestForms
{
    public class QF_Page : MonoBehaviour
    {
        [SerializeField] private int pageIndex;
        [SerializeField] private QF_QuestForm manager;

        private List<RectTransform> content;
        private int contentDisplayIndex;

        // UI elements
        [SerializeField, HideInInspector]private Button back, clear, next;
        [SerializeField, HideInInspector]private Button pageNext, pagePrevious;
        [SerializeField, HideInInspector]private TextMeshProUGUI pageCount;
        [SerializeField, HideInInspector]private TextMeshProUGUI header;
        [SerializeField, HideInInspector]private RectTransform contentTransform;

        public int PageNumber => pageIndex;

        private void Awake()
        {
            contentDisplayIndex = 0;

            SetupButtons();
        }

        public void ClearAnswers() 
        {
            var elements = GetComponentsInChildren<QF_PageElement>();
            foreach(var ele in elements) 
            {
                ele.Clear();
            }

            Debug.Log("Cleared Answers");
        }

        public void NextContentPage() 
        {
            Debug.Log("Next content Page");
        }

        public void PreviousContentPage() 
        {
            Debug.Log("Previous content page");
        }

        private void SetupButtons() 
        {
            Page page = manager.QuestSource.pages[pageIndex];
            int count = (page.questions.Length / QF_Rules.QuestionsPerPage) + 1 ;

            back.onClick.AddListener(manager.PreviousPage);
            next.onClick.AddListener(manager.NextPage);
            clear.onClick.AddListener(ClearAnswers);

            if (page.scrollQuestions == ScrollType.SplitToPage)
            {
                pagePrevious.onClick.AddListener(PreviousContentPage);
                pageNext.onClick.AddListener(NextContentPage);
                pageCount.text = $"{1} - {count}";
            }
        }

        /// <summary>
        /// Initializes the page
        /// </summary>
        /// <param name="i"> Index of the page in the questionnaire</param>
        /// <param name="manager"> Form manager that this page belongs to</param>
        public void Init(int i, QF_QuestForm manager)
        {
            pageIndex = i;
            this.manager = manager;
            Page page = manager.QuestSource.pages[i];

            // etc...
            GetReferences(page);
            SetupPage(page);
        }

        /// <summary>
        /// Big function to retrieve references from UI per page
        /// </summary>
        public void GetReferences(Page page) 
        {
            // Bottom form control
            back = transform.Find("QuestionnaireControl/Back").GetComponent<Button>();
            clear = transform.Find("QuestionnaireControl/Clear").GetComponent<Button>();
            next = transform.Find("QuestionnaireControl/Continue").GetComponent<Button>();

            if (page.scrollQuestions != ScrollType.SplitToPage)
            {
                transform.Find("PageControl").gameObject.SetActive(false);
            }
            else 
            {
                // Page control
                pageNext = transform.Find("PageControl/Next").GetComponent<Button>();
                pagePrevious = transform.Find("PageControl/Previous").GetComponent<Button>();
                pageCount = transform.Find("PageControl/Page Number").GetComponent<TextMeshProUGUI>();
            }

            header = transform.Find("BGPannel/Header/HeaderText").GetComponent<TextMeshProUGUI>();
            contentTransform = transform.Find("Content") as RectTransform;
        }

        public void SetupPage(Page page) 
        {
            RectTransform content;
            GameObject textContentLeght = Resources.Load<GameObject>("QF_PageLengthText");
            GameObject sectionHeader = Resources.Load<GameObject>("QF_SectionHeader");
            GameObject scaleElement = Resources.Load<GameObject>("QF_ScaleElement");

            // Page control
            header.text = page.header;

            if (page.scrollQuestions == ScrollType.Scroll)
            {
                GameObject scroll = Resources.Load<GameObject>("QF_ScrollPrefab");
                scroll = Instantiate(scroll, contentTransform);

                ScrollRect bar = scroll.GetComponent<ScrollRect>();
                content = bar.content;

                ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
                fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
            else 
            {
                content = contentTransform;
            }

            // Setup vertical group
            var group = content.gameObject.AddComponent<VerticalLayoutGroup>();
            group.padding = new RectOffset(5, 5, 5, 10);
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childScaleHeight = true;
            group.spacing = 5;
            group.childForceExpandHeight = false;

            var instructionsHeader = Instantiate(sectionHeader, content).GetComponent<TextMeshProUGUI>();
            instructionsHeader.text = "Instruções:";
            var instructions = Instantiate(textContentLeght, content).GetComponent<TextMeshProUGUI>();
            instructions.text = "   " + page.instructions;

            bool scaleQuestion = false;
            QF_Scale scaleGroup = null;
            for (int i = 0; i < page.questions.Length; i++)
            {
                Question q = page.questions[i];
                if (q.type == QuestionType.Scale) 
                {
                    if (!scaleQuestion)
                    {
                        // Instantiate scale
                        GameObject scale = Resources.Load<GameObject>("QF_ScaleElement");
                        scale = Instantiate(scale, content);
                        scaleGroup = scale.AddComponent<QF_Scale>();
                        scaleGroup.SetScale(page.scale);
                    }

                    if (scaleGroup != null) scaleGroup.AddQuestion(q);
                    scaleQuestion = true;
                }
                else 
                {
                    scaleQuestion = false;
                }

            }
        }
    }
}