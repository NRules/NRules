using System.Collections.Generic;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface IRuleSet
    {
        IRuleBuilder AddRule();
    }

    internal class RuleSet : IRuleSet
    {
        private readonly List<CompiledRule> _rules = new List<CompiledRule>();

        public IRuleBuilder AddRule()
        {
            var rule = new CompiledRule();
            _rules.Add(rule);
            return new RuleBuilder(rule);
        }

        internal IEnumerable<CompiledRule> Rules
        {
            get { return _rules; }
        }
    }
}