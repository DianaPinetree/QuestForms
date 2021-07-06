using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuestForms
{
    public class ExporterListAttribute : PropertyAttribute
    {
        public string[] Exporters => ExporterManager.Instance.ExporterList;
    }
}
