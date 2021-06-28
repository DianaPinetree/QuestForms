using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace QuestForms
{
    public class QF_Scale : MonoBehaviour
    {
        private List<Question> questions = new List<Question>();
        private Transform scaleHeader;
        private GameObject scaleTextObject;

        private int choices = -1;
        public int ScaleChoices => choices;

        private void Awake()
        {
            choices = transform.Find("Scale").childCount;
        }

        /// <summary>
        /// Adds a question to this group scale question
        /// </summary>
        /// <param name="q">Question to add</param>
        public void AddQuestion(Question q) 
        {
            if (choices == -1)
            {
                Debug.LogWarning("QF_Scale: you are calling AddQuestion before setting the scale, this will lead to errors");
            }

            GameObject prefab = Resources.Load<GameObject>("QF_ScaleQuestionGroup");
            GameObject question = Instantiate(prefab, transform);
            var ele = question.AddComponent<QF_ScaleQuestion>();

            ele.QuestionText.text = q.question;
            ele.AddToggles(choices);
            questions.Add(q);
            Instantiate(QF_Rules.Seperator, transform);
        }

        public void SetScale(string[] scale) 
        {
            if (scale == null) return;
            scaleHeader = transform.Find("Scale");
            scaleTextObject = scaleHeader.GetChild(0).gameObject;

            choices = scale.Length;
            foreach(string s in scale) 
            {
                GameObject obj = Instantiate(scaleTextObject, scaleHeader);
                var text = obj.GetComponent<TextMeshProUGUI>();
                text.text = s;
            }

            DestroyImmediate(scaleTextObject);
            scaleTextObject = scaleHeader.GetChild(0).gameObject;

            Instantiate(QF_Rules.Seperator, transform);
        }
    }
}
