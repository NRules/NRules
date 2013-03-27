using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public interface ICompiledRule
    {
        string Handle { get; }
        string Name { get; }
        int Priority { get; }
        IEnumerable<Declaration> Declarations { get; }
        IEnumerable<RuleAction> Actions { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        private readonly HashSet<Declaration> _declarations;
        private readonly IList<RuleAction> _actions;

        public static int DefaultPriority
        {
            get { return 0; }
        }

        public CompiledRule()
        {
            Name = string.Empty;
            Priority = DefaultPriority;
            Handle = Guid.NewGuid().ToString();
            _declarations = new HashSet<Declaration>();
            _actions = new List<RuleAction>();
        }

        public string Handle { get; private set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public IEnumerable<Declaration> Declarations
        {
            get { return _declarations; }
        }

        public IEnumerable<RuleAction> Actions
        {
            get { return _actions; }
        }

        public void AddDeclaration(Declaration declaration)
        {
            _declarations.Add(declaration);
        }

        public void AddAction(RuleAction action)
        {
            _actions.Add(action);
        }
    }
}