using System.Collections.Generic;

namespace NRules.Rule
{
    public interface IRuleSet
    {
        IRuleBuilder AddRule();
        IEnumerable<ICompiledRule> Rules { get; }
    }

    public class RuleSet : IRuleSet
    {
        private readonly List<CompiledRule> _rules = new List<CompiledRule>();

        public IRuleBuilder AddRule()
        {
            var rule = new CompiledRule();
            _rules.Add(rule);
            return new RuleBuilder(rule);
        }

        public IEnumerable<ICompiledRule> Rules
        {
            get { return _rules; }
        }
    }
}