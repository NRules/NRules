using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    internal interface ICompiledRule
    {
        int Priority { get; }
        RuleRepeatability Repeatability { get; }
        IRuleDefinition Definition { get; }
        IEnumerable<IRuleAction> Actions { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        private readonly int _priority;
        private readonly RuleRepeatability _repeatability;
        private readonly IRuleDefinition _definition;
        private readonly List<IRuleAction> _actions;

        public CompiledRule(IRuleDefinition definition, IEnumerable<IRuleAction> actions)
        {
            _priority = definition.Priority;
            _repeatability = definition.Repeatability;
            _definition = definition;
            _actions = new List<IRuleAction>(actions);
        }

        public int Priority
        {
            get { return _priority; }
        }

        public RuleRepeatability Repeatability
        {
            get { return _repeatability; }
        }

        public IRuleDefinition Definition
        {
            get { return _definition; }
        }

        public IEnumerable<IRuleAction> Actions
        {
            get { return _actions; }
        }
    }
}