using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class CompiledRule
    {
        private readonly HashSet<IDeclaration> _declarations = new HashSet<IDeclaration>();
        private readonly List<IPredicate> _predicates = new List<IPredicate>();
        private readonly List<IRuleAction> _actions = new List<IRuleAction>();

        public CompiledRule(string name)
        {
            Handle = Guid.NewGuid().ToString();
            Name = name;
        }

        public string Handle { get; private set; }
        public string Name { get; private set; }

        public ISet<IDeclaration> Declarations
        {
            get { return _declarations; }
        }

        public IList<IPredicate> Predicates
        {
            get { return _predicates; }
        }

        public IList<IRuleAction> Actions
        {
            get { return _actions; }
        }
    }
}