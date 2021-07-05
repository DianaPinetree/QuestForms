using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace QuestForms
{
    /// <summary>
    /// Helper class that combines given questions and headers and displays them
    /// nicely aligned in the UI
    /// 
    /// QuestionGroup enabled the possibility of reordering groups, randomizing
    /// the question order without changing the order of the questions in the
    /// exported answers file
    /// </summary>
    public class QF_QuestionGroup : QF_PageElement
    {
        [SerializeField] private List<QF_PageElement> elements = new List<QF_PageElement>();
        [SerializeField] private TextMeshProUGUI headerText;
        public Image backdrop;

        private void Awake()
        {
            backdrop = GetComponent<Image>();
            foreach(var ele in elements) 
            {
                Mandatory |= ele.Mandatory;
            }
        }

        public void AddElement(GameObject obj)
        {
            obj.transform.SetParent(transform);

            if (obj.TryGetComponent<QF_PageElement>(out var ele))
            {
                elements.Add(ele);
                Mandatory |= ele.Mandatory;
            }

            if (obj.TryGetComponent<TextMeshProUGUI>(out var t) && headerText == null)
            {
                headerText = t;
            }
        }
        public void AddElement(GameObject obj, int childIndex)
        {
            AddElement(obj);
            obj.transform.SetSiblingIndex(childIndex);
        }

        public void SetQuestionIndex()
        {
            string replace = transform.GetSiblingIndex().ToString();
            string original = headerText.text;
            int index = original.IndexOf('.');
            original = original.Remove(index - 1, 1).Insert(index - 1, replace);
            headerText.text = original;
        }

        public override bool Valid()
        {
            if (!Mandatory) return true;
            bool valid = true;
            foreach (QF_PageElement element in elements)
            {
                Debug.Log(element.Valid());
                valid &= element.Valid();
            }

            backdrop.enabled = !valid;
            return valid;
        }

        public override void Clear()
        {
            
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            backdrop = GetComponent<Image>();
            backdrop.enabled = false;
        }
#endif
    }
}
