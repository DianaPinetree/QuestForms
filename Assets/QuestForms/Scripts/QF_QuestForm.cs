using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestForms
{
    public class QF_QuestForm : MonoBehaviour
    {
        [SerializeField] private QF_Questionnaire questionnaire;

        private void Awake()
        {
            PrintData();
        }

        public void LoadQuestionnaire() 
        {
            // Cleanup Existing
            CleanUp();

            // Setup page structure
            SetupStructure();

            // Create questions
            CreateQuestions();
        }

        public void CleanUp()
        {
        
        }

        public void SetupStructure()
        {
        
        }

        public void CreateQuestions()
        {
        
        }

        private void PrintData() 
        {
            foreach(var page in questionnaire.pages) 
            {
                Debug.Log($"Page {page.ID}");
                Debug.Log(page.scrollQuestions);
                foreach(var question in page.questions) 
                {
                    
                }
            }
        }
    }
}
