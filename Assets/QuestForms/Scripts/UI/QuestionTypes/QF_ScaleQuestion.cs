using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    /// <summary>
    /// A unique line in a scale group questin
    /// </summary>
    public class QF_ScaleQuestion : QF_PageElement
    {
        private TextMeshProUGUI questionText;
        private ToggleGroup group;
        private QF_Scale scale;
        [SerializeField] private List<Toggle> toggles;
        public int Answer { get; private set; }

        public ToggleGroup Toggles
        {
            get
            {
                if (group == null)
                {
                    group = GetComponentInChildren<ToggleGroup>();
                }

                return group;
            }
        }
        public TextMeshProUGUI QuestionText
        {
            get
            {
                if (questionText == null)
                {
                    questionText = GetComponentInChildren<TextMeshProUGUI>();
                }

                return questionText;
            }
        }

        private void Awake()
        {
            scale = GetComponentInParent<QF_Scale>();

            for (int i = 0; i < toggles.Count; i++)
            {
                Toggle t = toggles[i];
                toggles[i].onValueChanged.AddListener(delegate { ToggleSelected(t); });
                t.isOn = false;
            }
        }

        /// <summary>
        /// Toggle callback
        /// </summary>
        /// <param name="selected">The selected toggle on this callback</param>
        public void ToggleSelected(Toggle selected) 
        {
            if (!selected.isOn) 
            {
#if UNITY_EDITOR
                gameObject.name = string.Format("QF_SQuestion", Answer);
#endif
                return;
            }
            int choices = scale.ScaleChoices;

            for (int i = 0; i < choices; i++)
            {
                if (Toggles.transform.GetChild(i).gameObject == selected.gameObject) 
                {
                    Answer = i;
#if UNITY_EDITOR
                    gameObject.name = string.Format("QF_SQuestion: Answer {0}", Answer);
#endif
                }
            }
        }

        /// <summary>
        /// Creates count amount of toggles for this scale question EDITOR ONLY
        /// </summary>
        /// <param name="count"> Number of toggles to create</param>
        public void AddToggles(int count)
        {
            toggles = new List<Toggle>();
            questionText = GetComponentInChildren<TextMeshProUGUI>();
            GameObject togglePrefab = Toggles.transform.GetChild(0).gameObject;
            togglePrefab.GetComponent<Toggle>().group = Toggles;

            for (int i = 0; i < count; i++)
            {
                var obj = Instantiate(togglePrefab, Toggles.transform);
                var togl = obj.GetComponent<Toggle>();
                togl.group = Toggles;
                togl.isOn = false;
                toggles.Add(togl);
            }

            Toggles.SetAllTogglesOff();
            DestroyImmediate(togglePrefab);
        }

        public override bool Valid()
        {
            if (!Mandatory) return true;
            
            return Toggles.AnyTogglesOn();
        }

        public override void Clear()
        {
            Toggles.SetAllTogglesOff();
        }
    }
}
