using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace QuestForms
{
    public class QF_Questionnaire : ScriptableObject
    {
        public Page[] pages;
        public List<QF_ImagePair> images;
    }

    [System.Serializable]
    public class QuestionnaireElement
    {
        public string ID;
        public bool containsImage;
    }

    [System.Serializable]
    public class Page : QuestionnaireElement
    {
        public string header;
        public string instructions;
        [NonReorderable]
        public string[] scale;
        [NonReorderable]
        public Question[] questions;
    }

    [System.Serializable]
    public class Question : QuestionnaireElement
    {
        public string qType; // Question type Scale, Option
        public string question;
        public bool mandatory;
        public QuestionType type => (QuestionType)Enum.Parse(typeof(QuestionType), qType);
    }

    public enum QuestionType
    {
        Scale,
        Option,
        TextField
    }
}
