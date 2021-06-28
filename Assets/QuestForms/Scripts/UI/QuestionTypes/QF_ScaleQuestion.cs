using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace QuestForms
{
    public class QF_ScaleQuestion : MonoBehaviour
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
            }

            Toggles.SetAllTogglesOff();
        }

        public void ToggleSelected(Toggle selected) 
        {
            if (!selected.isOn) return;
            int choices = scale.ScaleChoices;

            for (int i = 0; i < choices; i++)
            {
                if (group.transform.GetChild(i).gameObject == selected.gameObject) 
                {
                    Answer = i;
#if UNITY_EDITOR
                    gameObject.name = string.Format("QF_SQuestion: Answer {0}", Answer);
#endif
                }
            }
        }

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
                toggles.Add(togl);
            }

            Toggles.SetAllTogglesOff();
            DestroyImmediate(togglePrefab);
        }
    }
}
