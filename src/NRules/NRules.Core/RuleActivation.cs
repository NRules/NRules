using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    internal class RuleActivation
    {
        public RuleActivation(CompiledRule rule, Tuple tuple)
        {
            Rule = rule;
            Tuple = tuple;
        }

        public CompiledRule Rule { get; private set; }
        public Tuple Tuple { get; private set; }
    }
}