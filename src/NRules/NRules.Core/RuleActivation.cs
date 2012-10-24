using NRules.Core.Rete;
using NRules.Rule;

namespace NRules.Core
{
    internal class RuleActivation
    {
        public RuleActivation(ICompiledRule rule, Tuple tuple)
        {
            Rule = rule;
            Tuple = tuple;
        }

        public ICompiledRule Rule { get; private set; }
        public Tuple Tuple { get; private set; }
    }
}