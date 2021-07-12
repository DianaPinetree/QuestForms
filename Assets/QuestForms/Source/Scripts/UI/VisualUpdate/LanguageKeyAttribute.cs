using UnityEngine;
using System.Linq;

namespace QuestForms.Internal
{
    public class LanguageKeyAttribute : PropertyAttribute
    {
        public string[] Keys => QF_Rules.Instance.language.LanguageTable.Keys.ToArray();
    }
}