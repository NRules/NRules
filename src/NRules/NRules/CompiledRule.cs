using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    internal interface ICompiledRule
    {
        RuleRepeatability Repeatability { get; }
        IRuleDefinition Definition { get; }
        IRulePriority Priority { get; }
        IEnumerable<IRuleAction> Actions { get; }
        IEnumerable<IRuleDependency> Dependencies { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        private readonly RuleRepeatability _repeatability;
        private readonly IRuleDefinition _definition;
        private readonly IRulePriority _priority;
        private readonly List<IRuleAction> _actions;
        private readonly List<IRuleDependency> _dependencies;

        public CompiledRule(IRuleDefinition definition, IRulePriority priority, IEnumerable<IRuleAction> actions, IEnumerable<IRuleDependency> dependencies)
        {
            _repeatability = definition.Repeatability;
            _definition = definition;
            _priority = priority;
            _actions = new List<IRuleAction>(actions);
            _dependencies = new List<IRuleDependency>(dependencies);
        }

        public RuleRepeatability Repeatability
        {
            get { return _repeatability; }
        }

        public IRuleDefinition Definition
        {
            get { return _definition; }
        }

        public IRulePriority Priority
        {
            get { return _priority; }
        }

        public IEnumerable<IRuleAction> Actions
        {
            get { return _actions; }
        }

        public IEnumerable<IRuleDependency> Dependencies
        {
            get { return _dependencies; }
        }
    }
}