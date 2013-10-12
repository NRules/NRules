using System.Collections.Generic;

namespace NRules.Rule
{
    public interface IRuleSet
    {
        void AddRule(IRuleDefinition ruleDefinition);
        IEnumerable<IRuleDefinition> Rules { get; }
    }

    public class RuleSet : IRuleSet
    {
        private readonly List<IRuleDefinition> _rules = new List<IRuleDefinition>();

        public void AddRule(IRuleDefinition ruleDefinition)
        {
            _rules.Add(ruleDefinition);
        }

        public IEnumerable<IRuleDefinition> Rules
        {
            get { return _rules; }
        }
    }
}