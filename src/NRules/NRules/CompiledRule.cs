using System.Collections.Generic;
using System.Diagnostics;
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
        IEnumerable<IRuleDependency> Dependencies { get; }
        IRuleFilter Filter { get; }
        ActionTrigger ActionTriggers { get; }
    }

    [DebuggerDisplay("{Definition.Name}")]
    internal class CompiledRule : ICompiledRule
    {
        private readonly List<Declaration> _declarations;
        private readonly List<IRuleAction> _actions;
        private readonly List<IRuleDependency> _dependencies;

        public CompiledRule(IRuleDefinition definition, IEnumerable<Declaration> declarations, IEnumerable<IRuleAction> actions, IEnumerable<IRuleDependency> dependencies, IRuleFilter filter)
        {
            Priority = definition.Priority;
            Repeatability = definition.Repeatability;
            Definition = definition;
            Filter = filter;
            _declarations = new List<Declaration>(declarations);
            _actions = new List<IRuleAction>(actions);
            _dependencies = new List<IRuleDependency>(dependencies);

            foreach (var ruleAction in _actions)
            {
                ActionTriggers |= ruleAction.Trigger;
            }
        }

        public int Priority { get; }
        public RuleRepeatability Repeatability { get; }
        public IRuleDefinition Definition { get; }
        public IRuleFilter Filter { get; }
        public ActionTrigger ActionTriggers { get; }

        public IEnumerable<Declaration> Declarations => _declarations;
        public IEnumerable<IRuleAction> Actions => _actions;
        public IEnumerable<IRuleDependency> Dependencies => _dependencies;
    }
}