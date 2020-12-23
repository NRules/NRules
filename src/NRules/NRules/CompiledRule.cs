using System.Collections.Generic;
using System.Diagnostics;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules
{
    internal interface ICompiledRule
    {
        int Priority { get; }
        RuleRepeatability Repeatability { get; }
        IRuleDefinition Definition { get; }
        IEnumerable<Declaration> Declarations { get; }
        IEnumerable<IRuleAction> Actions { get; }
        IRuleFilter Filter { get; }
        ActionTrigger ActionTriggers { get; }
        IndexMap FactMap { get; }
    }

    [DebuggerDisplay("{Definition.Name}")]
    internal class CompiledRule : ICompiledRule
    {
        private readonly List<Declaration> _declarations;
        private readonly List<IRuleAction> _actions;

        public CompiledRule(IRuleDefinition definition, IEnumerable<Declaration> declarations, IEnumerable<IRuleAction> actions, IRuleFilter filter, IndexMap factMap)
        {
            Definition = definition;
            Filter = filter;
            FactMap = factMap;
            _declarations = new List<Declaration>(declarations);
            _actions = new List<IRuleAction>(actions);

            foreach (var ruleAction in _actions)
            {
                ActionTriggers |= ruleAction.Trigger;
            }
        }

        public int Priority => Definition.Priority;
        public RuleRepeatability Repeatability => Definition.Repeatability;
        public IRuleDefinition Definition { get; }
        public IRuleFilter Filter { get; }
        public ActionTrigger ActionTriggers { get; }
        public IndexMap FactMap { get; }

        public IEnumerable<Declaration> Declarations => _declarations;
        public IEnumerable<IRuleAction> Actions => _actions;
    }
}