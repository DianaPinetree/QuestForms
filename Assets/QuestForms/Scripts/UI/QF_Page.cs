using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace QuestForms
{
    public class QF_Page : MonoBehaviour
    {
        private static GameObject contentWideText;
        [SerializeField] private int pageIndex;
        [SerializeField] private QF_QuestForm manager;
        [SerializeField] private List<QF_QuestionGroup> questionGroups;
        [SerializeField, HideInInspector] bool shuffleContents;
        [SerializeField, HideInInspector] private List<RectTransform> content;
        private int contentDisplayIndex;
        public bool RandomizeContent => shuffleContents;

        // UI elements
        [SerializeField, HideInInspector] private Button back, clear, next;
        [SerializeField, HideInInspector] private Button pageNext, pagePrevious;
        [SerializeField, HideInInspector] private TextMeshProUGUI pageCount;
        [SerializeField, HideInInspector] private TextMeshProUGUI header;
        [SerializeField, HideInInspector] private TextMeshProUGUI pageMessage;
        [SerializeField, HideInInspector] private RectTransform contentTransform;

        public int PageNumber => pageIndex;

        private void Awake()
        {
            contentDisplayIndex = 0;

            SetupButtons();
        }

        public void ClearAnswers()
        {
            var elements = GetComponentsInChildren<QF_PageElement>();
            foreach (var ele in elements)
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
            int count = (QF_Rules.QuestionsPerPage / page.questions.Length);

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

        public void PageMessage(string message)
        {
            pageMessage.text = message;
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
            GameObject pageControl = transform.Find("PageControl").gameObject;
            if (page.scrollQuestions != ScrollType.SplitToPage || (QF_Rules.QuestionsPerPage / page.questions.Length <= 1))
            {
                pageControl.SetActive(false);
            }
            else
            {
                // Page control
                pageNext = transform.Find("PageControl/Next").GetComponent<Button>();
                pagePrevious = transform.Find("PageControl/Previous").GetComponent<Button>();
                pageCount = transform.Find("PageControl/Page Number").GetComponent<TextMeshProUGUI>();
            }

            header = transform.Find("BGPannel/Header/HeaderText").GetComponent<TextMeshProUGUI>();
            pageMessage = transform.Find("Page Message").GetComponent<TextMeshProUGUI>();
            contentTransform = transform.Find("Content") as RectTransform;
        }

        /// <summary>
        /// Creates a content header field with a body text, used for questions and instructions in a page
        /// </summary>
        /// <param name="content">Content transform to create the header in</param>
        /// <param name="header">Header text</param>
        /// <param name="body">body text</param>
        /// <param name="breakParts">Adds a line break in between the header and the body</param>
        public static GameObject ContentText(Transform content, string header = "", string body = "", bool breakParts = false)
        {
            if (contentWideText == null)
            {
                contentWideText = Resources.Load<GameObject>("QF_PageLengthText");
            }

            GameObject o = Instantiate(contentWideText, content);
            var textContent = o.GetComponent<TextMeshProUGUI>();
            textContent.text = $"<size={QF_Rules.HeaderFontSize}><b>{header}</b></size>";
            if (breakParts) textContent.text += "\n";
            textContent.text += $"<size={QF_Rules.QuestionFontSize}>{body}</size>";

            return o;
        }
        public static string ContentText(string header = "", string body = "", bool breakParts = false)
        {
            string text;
            text = $"<size={QF_Rules.HeaderFontSize}><b>{header}</b></size>";
            if (breakParts) text += "\n";
            text += $"<size={QF_Rules.QuestionFontSize}>{body}</size>";

            return text;
        }

        /// <summary>
        /// Creates a page, this is where most of the UI generation resides
        /// </summary>
        /// <param name="page"> Page data to generate</param>
        public void SetupPage(Page page)
        {
            RectTransform content;
            GameObject scaleElement = Resources.Load<GameObject>("QF_ScaleElement");

            questionGroups = new List<QF_QuestionGroup>();
            shuffleContents = page.randomizeOrder;
            // Page control
            header.text = page.header;
            pageMessage.text = string.Empty;

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
            group.spacing = QF_Rules.QuestionSpacing;
            group.childForceExpandHeight = false;

            if (!string.IsNullOrEmpty(page.instructions))
            {
                ContentText(content, "Instruções: ", page.instructions);
            }

            bool scaleQuestion = false;
            QF_Scale scaleGroup = null;
            int questionIndex = 0;
            GameObject g = Resources.Load<GameObject>("QF_QuestionGroup");
            GameObject imagePrefab = Resources.Load<GameObject>("QF_Image");
            QF_QuestionGroup qGroup = null;

            for (int i = 0; i < page.questions.Length; i++)
            {
                Question q = page.questions[i];

                if (q.type == QuestionType.Scale)
                {
                    if (!scaleQuestion)
                    {
                        questionIndex++;

                        // Instantiate scale
                        GameObject scale = Resources.Load<GameObject>("QF_ScaleElement");
                        GameObject questionText = ContentText(content, $"{questionIndex}. ");

                        scale = Instantiate(scale, content);
                        scaleGroup = scale.AddComponent<QF_Scale>();
                        scaleGroup.SetScale(page.scale);

                        qGroup = Instantiate(g, content).GetComponent<QF_QuestionGroup>();
                        qGroup.AddElement(questionText);
                        qGroup.AddElement(scale);
                    }

                    if (scaleGroup != null)
                    {
                        QF_ScaleQuestion qF_ScaleQuestion = scaleGroup.AddQuestion(q);
                        qF_ScaleQuestion.ID = q.ID;
                    } 
                    scaleQuestion = true;
                }
                else
                {
                    questionIndex++;
                    qGroup = Instantiate(g, content).GetComponent<QF_QuestionGroup>();
                    GameObject questionText = ContentText(content, $"{questionIndex}. ", q.question);
                    qGroup.AddElement(questionText);
                }

                if (q.type == QuestionType.TextField)
                {
                    GameObject field = Resources.Load<GameObject>("QF_TextField");
                    field = Instantiate(field, content);
                    QF_TextField textField = field.AddComponent<QF_TextField>();
                    textField.Mandatory = q.mandatory;
                    textField.SetCharacterLimits(q.characterMin, q.characterMax);
                    qGroup?.AddElement(field);

                    textField.ID = q.ID;
                }

                if (q.type == QuestionType.Option)
                {
                    GameObject op = new GameObject("QF_Option");
                    op.transform.SetParent(content);
                    op.transform.localScale = Vector3.one;
                    QF_OptionGroup opgroup = op.AddComponent<QF_OptionGroup>();
                    
                    // Add question
                    opgroup.AddQuestion(q);

                    opgroup.ID = q.ID;
                    qGroup?.AddElement(op);
                }

                if (q.containsImage)
                {
                    QF_ImagePair imgInfo = manager.QuestSource.GetImagePair(q.ID);
                    if(imgInfo.image != null)
                    {
                        GameObject questionnaireImage = Instantiate(imagePrefab, content);
                        var layout = questionnaireImage.GetComponent<LayoutGroup>();
                        var img = questionnaireImage.GetComponentInChildren<Image>();
                        
                        // Set image layout position
                        layout.childAlignment = imgInfo.alignment;

                        // Set Image ratio
                        float ratio = img.sprite.rect.height / img.sprite.rect.width;
                        img.sprite = imgInfo.image;
                        img.rectTransform.sizeDelta = new Vector2(300 / ratio, 300);

                        qGroup?.AddElement(questionnaireImage, (int)imgInfo.position + 1);
                    }
                }

                questionGroups.Add(qGroup);
            }

        }

        public void ShuffleContent()
        {
            Shuffle(questionGroups);

            for (int i = 0; i < questionGroups.Count; i++)
            {
                questionGroups[i].transform.SetSiblingIndex(i + 1);
            }
            for (int i = 0; i < questionGroups.Count; i++)
            {
                questionGroups[i].SetQuestionIndex();
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}