using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    internal interface ICompiledRule
    {
        int Priority { get; }
        IRuleDefinition Definition { get; }
        IEnumerable<IRuleAction> Actions { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        public CompiledRule(IRuleDefinition definition, IEnumerable<IRuleAction> actions)
        {
            Priority = definition.Priority;
            Definition = definition;
            Actions = new List<IRuleAction>(actions);
        }

        public int Priority { get; private set; }
        public IRuleDefinition Definition { get; private set; }
        public IEnumerable<IRuleAction> Actions { get; private set; }
    }
}