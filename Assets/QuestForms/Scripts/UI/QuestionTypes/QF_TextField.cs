using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace QuestForms
{

    public class QF_TextField : QF_PageElement
    {
        [SerializeField] private bool showCurrentCharacter = true;
        [SerializeField] private int baseLines = 5;

        private TMP_InputField inputField;
        [SerializeField] private int characterMin;
        [SerializeField] private int characterMax;

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
            inputField.characterLimit = characterMax;
        }

        public void SetCharacterLimits(int min, int max)
        {
            characterMin = min;
            characterMax = max;
            HideScrollBar();
        }

        public void ShowScrollBar() 
        {
            Scroll.gameObject.SetActive(true);
            inputField.verticalScrollbar = Scroll;
        }

        public void HideScrollBar()
        {
            Scroll.gameObject.SetActive(false);
            inputField.verticalScrollbar = null;
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
        }
#endif
    }
}

