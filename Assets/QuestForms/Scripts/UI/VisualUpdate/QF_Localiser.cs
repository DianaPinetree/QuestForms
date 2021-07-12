using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using QuestForms.Internal;

namespace QuestForms
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class QF_Localiser : MonoBehaviour
    {
        [SerializeField, LanguageKey] string languageKey = "";
        // Start is called before the first frame update
        private void Start()
        {
            UpdateTextLanguage();
        }

        public void OnValidate()
        {
            UpdateTextLanguage();
        }

        private void UpdateTextLanguage()
        {
            var t = GetComponent<TextMeshProUGUI>();
            t.text = QF_Rules.Instance.language[languageKey];
        }
    }
}