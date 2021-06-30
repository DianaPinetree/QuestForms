using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    public class QF_OptionGroup : QF_PageElement
    {
        [SerializeField] private List<QF_OptionElement> options = new List<QF_OptionElement>();
        private ToggleGroup toggles;
        public int Answer { get; private set; }

        private void Awake()
        {
            toggles = GetComponent<ToggleGroup>();
        }

        private void Start()
        {
            foreach (QF_OptionElement option in options)
            {
                option.Toggle.group = toggles;
                option.Toggle.onValueChanged.AddListener(delegate { ToggleSelected(option); });
            }

            toggles.SetAllTogglesOff();
            Answer = -1;
        }

        public override void Clear()
        {
            toggles.SetAllTogglesOff();
        }

        public override bool Valid()
        {
            if (!Mandatory) return true;
            return toggles.AnyTogglesOn();
        }

        public void ToggleSelected(QF_OptionElement selected)
        {
            if (!selected.Toggle.isOn) 
            {
#if UNITY_EDITOR
                gameObject.name = string.Format("QF_Option", Answer);
#endif
                return;
            }
            int choices = options.Count;

            for (int i = 0; i < choices; i++)
            {
                if (toggles.transform.GetChild(i).gameObject == selected.gameObject) 
                {
                    Answer = i;
#if UNITY_EDITOR
                    gameObject.name = string.Format("QF_Option: Answer {0}", Answer);
#endif
                }
            }
        }

#if UNITY_EDITOR
        public void AddQuestion(Question q)
        {
            if (q.options.Length <= 0)
            {
                Debug.LogError($"Question {q.ID} could not be generated, options are empty");
                enabled = false;
                return;
            }

            Mandatory = q.mandatory;
            // Create toggle group
            toggles = gameObject.AddComponent<ToggleGroup>();
            toggles.allowSwitchOff = true;



            // Load gameobject for each option
            GameObject prefab = Resources.Load<GameObject>("QF_OptionElement");

            for (int i = 0; i < q.options.Length; i++)
            {
                GameObject optionElement = Instantiate(prefab, transform);
                var ele = optionElement.AddComponent<QF_OptionElement>();
                options.Add(ele);
                ele.SetOptionText(q.options[i]);
            }

            HorizontalOrVerticalLayoutGroup layout;
            // Add Layout group
            if (q.optionsLayout == Layout.Vertical)
            {
                layout = gameObject.AddComponent<VerticalLayoutGroup>();


            }
            else
            {
                layout = gameObject.AddComponent<HorizontalLayoutGroup>();
            }

            layout.childControlHeight = true;
            layout.childControlWidth = true;
            layout.childScaleHeight = true;
            layout.childScaleWidth = true;
            layout.childForceExpandHeight = false;
            layout.childForceExpandWidth = false;

            layout.spacing = QF_Rules.OptionsSpacing;
            Canvas.ForceUpdateCanvases();
            layout.SetLayoutVertical();
            layout.SetLayoutHorizontal();

            AlignLayouts();
        }

        private void OnValidate()
        {
            AlignLayouts();
        }

        private void AlignLayouts()
        {
            float maxWidth = 0;
            List<LayoutElement> layouts = new List<LayoutElement>(options.Count);
            for (int i = 0; i < options.Count; i++)
            {
                var t = options[i].GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement l = t.gameObject.AddComponent<LayoutElement>();
                layouts.Add(l);
                if (t.rectTransform.rect.width > maxWidth)
                {
                    maxWidth = t.rectTransform.rect.width;
                }
            }

            for (int i = 0; i < layouts.Count; i++)
            {
                layouts[i].preferredWidth = maxWidth;
            }
        }
#endif
    }
}
