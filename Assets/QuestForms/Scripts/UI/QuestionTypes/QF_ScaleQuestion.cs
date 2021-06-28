using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    public class QF_ScaleQuestion : MonoBehaviour
    {
        private TextMeshProUGUI questionText;
        private ToggleGroup group;
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

        private GameObject togglePrefab;
        public void AddToggles(int count) 
        {
            questionText = GetComponentInChildren<TextMeshProUGUI>();
            togglePrefab = Toggles.transform.GetChild(0).gameObject;
            togglePrefab.GetComponent<Toggle>().group = Toggles;

            for (int i = 0; i < count - 1; i++)
            {
                var obj = Instantiate(togglePrefab, Toggles.transform);
                obj.GetComponent<Toggle>().group = Toggles;
            }
        }
    }
}
