using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace QuestForms
{

    public class QF_TextField : QF_PageElement, IAnswerElement
    {
        [SerializeField] private bool showCurrentCharacter = true;
        [SerializeField] private int baseLines = 5;
        [SerializeField] private int characterMin;
        [SerializeField] private int characterMax;

        public object Answer => inputField.text;
        [field: SerializeField] public string ID {get; set;}

        private TMP_InputField inputField;
        private TextMeshProUGUI characterCountField;
        private TextMeshProUGUI minimumCharacterField;

        private int currentCharacterCount;
        private Scrollbar scroll;
        private Scrollbar Scroll 
        {
            get 
            {
                if (scroll == null) 
                {
                    scroll = GetComponentInChildren<Scrollbar>(true);
                }

                return scroll;
            }
        }

        private void Awake()
        {
            inputField = GetComponentInChildren<TMP_InputField>();
            characterCountField = transform.Find("Footer/Character Count").GetComponent<TextMeshProUGUI>();
            minimumCharacterField = transform.Find("Footer/Minimum").GetComponent<TextMeshProUGUI>();
            
            inputField.characterLimit = characterMax;
            inputField.onValueChanged.AddListener(UpdateCharacterCount);
            characterCountField.text = currentCharacterCount.ToString() + "/" + characterMax.ToString();

            if (Mandatory) 
            {
                minimumCharacterField.text = $"(Minimo de {characterMin} Caracteres)";
            }
            else 
            {
                minimumCharacterField.text = "";
            }
        }

        public void UpdateCharacterCount(string text) 
        {
            currentCharacterCount = text.Length;
            characterCountField.text = currentCharacterCount.ToString() + "/" + characterMax.ToString();
        }

        public void SetCharacterLimits(int min, int max)
        {
            characterMin = min;
            characterMax = max;
            SetScrollbarVisibility(false);
        }

        public void SetScrollbarVisibility(bool value) 
        {
            Scroll.gameObject.SetActive(value);
            inputField.verticalScrollbar = value ? Scroll : null;
        }

        public override void Clear()
        {
            inputField.text = string.Empty;
        }

        public override bool Valid()
        {
            if (!Mandatory) return true;

            return inputField.text.Length >= characterMin;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            int height = QF_Rules.QuestionFontSize * baseLines;
            inputField = GetComponentInChildren<TMP_InputField>();
            var rt = (inputField.transform as RectTransform);
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);

            if (showCurrentCharacter) 
            {
                
            }
        }
#endif
    }
}

