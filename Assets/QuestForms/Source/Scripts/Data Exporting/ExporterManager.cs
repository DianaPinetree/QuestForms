using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace QuestForms
{
    public class ExporterManager
    {
        private static readonly Lazy<ExporterManager> instance =
            new Lazy<ExporterManager>(() => new ExporterManager());

        private readonly IDictionary<string, Type> exportersTable;

        public static ExporterManager Instance => instance.Value;
        public string[] ExporterList => exportersTable.Keys.ToArray();
        public Type this[string value] => exportersTable[value];
        private ExporterManager()
        {
            Type exporterInterface = typeof(IQuestionnaireExporter);

            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(s => s.GetTypes()).
                Where(p => exporterInterface.IsAssignableFrom(p) 
                    && p.GetConstructor(Type.EmptyTypes) != null
                    && !p.IsAbstract);
            
            exportersTable = types.ToDictionary(t => t.FullName, t => t);
        }
    }
}

