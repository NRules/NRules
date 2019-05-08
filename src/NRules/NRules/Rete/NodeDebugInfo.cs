using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class NodeDebugInfo
    {
        private readonly List<IRuleDefinition> _rules = new List<IRuleDefinition>();
        private readonly List<RuleElement> _elements = new List<RuleElement>();

        public IEnumerable<IRuleDefinition> Rules => _rules;
        public IEnumerable<RuleElement> Elements => _elements;

        public void Add(IRuleDefinition rule, RuleElement element)
        {
            _rules.Add(rule);
            _elements.Add(element);
        }
    }
}
