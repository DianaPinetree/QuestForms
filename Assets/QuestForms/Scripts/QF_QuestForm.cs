using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestForms
{
    public class QF_QuestForm : MonoBehaviour
    {
        [SerializeField] private QF_Questionnaire questionnaire;

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
    }
}
