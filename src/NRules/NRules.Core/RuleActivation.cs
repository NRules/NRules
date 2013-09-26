using NRules.Core.Rete;
using NRules.Rule;

namespace NRules.Core
{
    internal class RuleActivation
    {
        public RuleActivation(IRuleDefinition ruleDefinition, Tuple tuple)
        {
            RuleDefinition = ruleDefinition;
            Tuple = tuple;
        }

        public IRuleDefinition RuleDefinition { get; private set; }
        public Tuple Tuple { get; private set; }
    }
}