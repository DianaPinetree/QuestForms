using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace QuestForms
{
    public class QF_CSVExporter : IQuestionnaireExporter
    {
        public string Extension => ".csv";
        public string FormatData(IEnumerable<IAnswerElement> answers)
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach(IAnswerElement e in answers)
            {
                stringBuilder.Append(e.ID + ",");
            }

            stringBuilder.Append('\n');

            foreach(IAnswerElement e in answers)
            {
                stringBuilder.Append(e.Answer);
                stringBuilder.Append(",");
            }

            return stringBuilder.ToString();
        }
    }
}
