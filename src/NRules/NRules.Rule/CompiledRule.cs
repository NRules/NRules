using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public interface ICompiledRule
    {
        string Handle { get; }
        string Name { get; }
        int Priority { get; }
        ISet<Declaration> Declarations { get; }
        IEnumerable<IRuleAction> Actions { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        private readonly HashSet<Declaration> _declarations;
        private readonly IList<IRuleAction> _actions;

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
            _actions = new List<IRuleAction>();
        }

        public string Handle { get; private set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public ISet<Declaration> Declarations
        {
            get { return _declarations; }
        }

        public IEnumerable<IRuleAction> Actions
        {
            get { return _actions; }
        }

        public void AddAction(IRuleAction action)
        {
            _actions.Add(action);
        }
    }
}