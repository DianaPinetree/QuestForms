using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QuestForms
{

    [RequireComponent(typeof(Image))]
    public class QF_Color : MonoBehaviour
    {
        enum ColorElement
        {
            Primary,
            Secondary,
            Accent,
            Invalid,
            Background
        }
        [SerializeField] private ColorElement color;
        void Start()
        {
            PickColor();
        }

        public void OnValidate()
        {
            PickColor();
        }

        private void PickColor()
        {
            Image img = GetComponent<Image>();
            switch (color)
            {
                case ColorElement.Primary:
                    img.color = QF_Rules.Instance.primaryColor;
                    break;
                case ColorElement.Secondary:
                    img.color = QF_Rules.Instance.secondaryColor;
                    break;
                case ColorElement.Accent:
                    img.color = QF_Rules.Instance.accentColor;
                    break;
                case ColorElement.Background:
                    img.color = QF_Rules.Instance.backgroundColor;
                    break;
                case ColorElement.Invalid:
                    img.color = QF_Rules.Instance.invalidColor;
                    break;
            }

        }
    }
}

