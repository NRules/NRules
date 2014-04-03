using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Fluent
{
    internal class RuleSet : IRuleSet
    {
        private readonly List<IRuleDefinition> _rules = new List<IRuleDefinition>();

        public RuleSet(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public void Add(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            _rules.AddRange(ruleDefinitions);
        }

        public IEnumerable<IRuleDefinition> Rules
        {
            get { return _rules; }
        }
    }
}