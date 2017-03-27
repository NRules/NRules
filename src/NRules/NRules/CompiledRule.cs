using System.Collections.Generic;
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
    }

    internal class CompiledRule : ICompiledRule
    {
        private readonly int _priority;
        private readonly RuleRepeatability _repeatability;
        private readonly IRuleDefinition _definition;
        private readonly List<Declaration> _declarations;
        private readonly List<IRuleAction> _actions;
        private readonly List<IRuleDependency> _dependencies;

        public CompiledRule(IRuleDefinition definition, IEnumerable<Declaration> declarations, IEnumerable<IRuleAction> actions, IEnumerable<IRuleDependency> dependencies)
        {
            _priority = definition.Priority;
            _repeatability = definition.Repeatability;
            _definition = definition;
            _declarations = new List<Declaration>(declarations);
            _actions = new List<IRuleAction>(actions);
            _dependencies = new List<IRuleDependency>(dependencies);
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

        public IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
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