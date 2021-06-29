using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestForms
{
    public abstract class QF_PageElement : MonoBehaviour
    {
        [SerializeField] private bool mandatory;
        public bool Mandatory { get => mandatory; set => mandatory = value; }
        /// <summary>
        /// If this element is valid
        /// </summary>
        /// <returns> Valid</returns>
        public abstract bool Valid();
        public abstract void Clear();
    }
}
