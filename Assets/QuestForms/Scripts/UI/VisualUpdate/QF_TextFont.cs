using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace QuestForms
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class QF_TextFont : MonoBehaviour
    {
        enum TextType
        {
            Header,
            Body
        }

        [SerializeField] private TextType textElementType = TextType.Body;
        [SerializeField] private bool changeColor = true;
        private TextMeshProUGUI text;

        private void Start() 
        {
            UpdateFont();
        }

        public void OnValidate() 
        {
            UpdateFont();
        }

        public void UpdateFont()
        {
            text = GetComponent<TextMeshProUGUI>();
            if (textElementType == TextType.Header)
            {
                text.font = QF_Rules.HeaderFont;
            }
            else
            {
                text.font = QF_Rules.TextFont;
            }
            
            if (changeColor)
            {
                text.color = QF_Rules.Instance.textColor;
            }
        }
    }

}