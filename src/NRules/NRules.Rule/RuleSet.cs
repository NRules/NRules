using System.Collections.Generic;

namespace NRules.Rule
{
    public interface IRuleSet
    {
        IRuleBuilder AddRule();
        IEnumerable<IRuleDefinition> Rules { get; }
    }

    public class RuleSet : IRuleSet
    {
        private readonly List<RuleDefinition> _rules = new List<RuleDefinition>();

        public IRuleBuilder AddRule()
        {
            var rule = new RuleDefinition();
            _rules.Add(rule);
            return new RuleBuilder(rule);
        }

        public IEnumerable<IRuleDefinition> Rules
        {
            get { return _rules; }
        }
    }
}