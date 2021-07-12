using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    public class QF_ConfirmationPage : MonoBehaviour
    {
        private TextMeshProUGUI backButtonText;
        private TextMeshProUGUI finishButtonText;
        private Button backButton;
        private Button finishButton;
        private ToggleGroup toggles;
        private Toggle[] objectToggles;
        private Image backdrop;
        private int answer;
        public QF_QuestForm manager;

        private void Start() 
        {
            backButton = transform.Find("QuestionnaireControl/Back").GetComponent<Button>();
            finishButton  = transform.Find("QuestionnaireControl/Continue").GetComponent<Button>();

            toggles = transform.Find("Content/Data").GetComponentInChildren<ToggleGroup>();
            backdrop = transform.Find("Content/Data").GetComponent<Image>();

            objectToggles = toggles.GetComponentsInChildren<Toggle>();

            for (int i = 0; i < objectToggles.Length; i++)
            {
                Toggle t = objectToggles[i];
                objectToggles[i].onValueChanged.AddListener(delegate{GetChoice(t);});
            }

            backdrop.enabled = false;
            
            backButton.onClick.AddListener(manager.LastPage);
            backButton.onClick.AddListener(() => gameObject.SetActive(false));
            finishButton.onClick.AddListener(FinishQuestionnaire);
        }

        private void GetChoice(Toggle t)
        {
            if (!t.isOn) 
            {
                return;
            }

            for (int i = 0; i < objectToggles.Length; i++)
            {
                if (t.gameObject == objectToggles[i].gameObject) 
                {
                    answer = i;
                }
            }
        }

        private void FinishQuestionnaire()
        {
            bool anyTogglesOn = toggles.AnyTogglesOn();
            backdrop.enabled = !anyTogglesOn;

            if (anyTogglesOn)
            {
                if (answer == 0)
                {
                    manager.ExportAnswers();
                }

                manager.OnQuestionnaireEnd();
            }
        }

    }
}
