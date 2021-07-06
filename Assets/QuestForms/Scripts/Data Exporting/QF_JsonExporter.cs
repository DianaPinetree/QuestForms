using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

namespace QuestForms
{
    public class QF_JsonExporter : IQuestionnaireExporter
    {
        public string Extension => ".json";
        public string FormatData(IEnumerable<IAnswerElement> answers)
        {
            StringBuilder data = new StringBuilder();
            IAnswerElement[] answerArray = answers.ToArray();

            data.Append("{\"questionnaire\":{");
            data.Append("\"answers\": [");
            
            for (int i = 0; i < answerArray.Length; i++)
            {
                IAnswerElement e = answerArray[i];

                string answerString = e.Answer == null ? "" : e.Answer.ToString();
                // New object block with ID of the question and the corresponding answer
                data.Append($"{{\"id\": \"{e.ID}\", \"answer\":\"{answerString}\"}}");
                
                if (i < answerArray.Length - 1)
                {
                    // Add a comma to separate objects
                    data.Append(",");
                }
            }

            // Close json
            data.Append("]");
            data.Append("} }");


            return data.ToString();
        }
    }
}
