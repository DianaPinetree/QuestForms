using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    public class QF_OptionElement : MonoBehaviour
    {
        private TextMeshProUGUI optionText;
        public Toggle Toggle { get; private set; }

        private void Awake()
        {
            Toggle = GetComponentInChildren<Toggle>();
            optionText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetOptionText(string text)
        {
            optionText = GetComponentInChildren<TextMeshProUGUI>();
            optionText.text = text;
        }
    }
}

