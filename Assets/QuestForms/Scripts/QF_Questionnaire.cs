using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class QF_Questionnaire : ScriptableObject
{
    public Page[] pages;
    public List<QF_ImagePair> images;
}

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
    public string[] scale;
    public Question[] questions;
}

[System.Serializable]
public class Question : QuestionnaireElement 
{
    public string typeName;
    public string question;
    public bool mandatory;
    public QuestionType type => (QuestionType)Enum.Parse(typeof(QuestionType), typeName);
}

public enum QuestionType 
{
    Scale,
    Option,
}