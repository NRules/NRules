using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal class CompiledRule
    {
        public CompiledRule(string name)
        {
            Name = name;
            Handle = Guid.NewGuid().ToString();
            Declarations = new HashSet<IDeclaration>();
            Predicates = new List<IPredicate>();
            Conditions = new List<ICondition>();
            Actions = new List<IRuleAction>();
        }

        public string Handle { get; private set; }
        public string Name { get; private set; }

        public ISet<IDeclaration> Declarations { get; private set; }
        public IList<IPredicate> Predicates { get; private set; }
        public IList<ICondition> Conditions { get; private set; }
        public IList<IRuleAction> Actions { get; private set; }
    }
}