using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestForms
{
    public interface IQuestionnaireExporter
    {
        string Extension {get;}
        string FormatData(IEnumerable<IAnswerElement> answers);
    }
}
