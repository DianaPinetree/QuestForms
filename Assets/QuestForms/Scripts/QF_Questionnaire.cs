using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace QuestForms
{
    public class QF_Questionnaire : ScriptableObject
    {
        public Page[] pages;
        public int numberOfImages;
        public List<QF_ImagePair> images = new List<QF_ImagePair>();

        public bool ContainsImage(string id)
        {
            return images.Any((x) => x.Equals(id));
        }

        // Index of the image pair corresponding to the ID
        public int ImagePair(string id)
        {
            if (!ContainsImage(id)) return -1;
            return images.FindIndex((x) => x.id == id);
        }

        public void CreateImages()
        {
            // Create images
            // Set Images
            if (pages == null) return;

            for (int i = 0; i < pages.Length; i++)
            {
                Page page = pages[i];
                if (page.containsImage && !ContainsImage(page.ID))
                {
                    images.Add(new QF_ImagePair(page.ID));
                }

                for (int q = 0; q < page.questions.Length; q++)
                {
                    Question question = page.questions[q];

                    if (question.containsImage && !ContainsImage(question.ID))
                    {
                        images.Add(new QF_ImagePair(question.ID));
                    }
                }
            }
            numberOfImages = images.Count;
        }
    }

    [System.Serializable]
    public class Page
    {
        public string ID;
        public bool containsImage;
        public string header;
        public string instructions;
        public string[] scale;
        public Question[] questions;
        public bool randomizeOrder;
        public ScrollType scrollQuestions;
    }

    [System.Serializable]
    public class Question
    {
        public string ID;
        public bool containsImage;
        public string qType; // Question type Scale, Option, TextField
        public string question;
        public bool mandatory;
        public string[] options;
        public Layout optionsLayout;
        public int characterMax = 200;
        public int characterMin = 50;
        public QuestionType type => (QuestionType)Enum.Parse(typeof(QuestionType), qType);
    }

    public enum Layout
    {
        Vertical,
        Horizontal
    }

    public enum QuestionType
    {
        Scale,
        Option,
        TextField
    }

    public enum ScrollType
    {
        Scroll,
        SplitToPage
    }
}
