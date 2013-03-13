using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public interface ICompiledRule
    {
        string Handle { get; }
        string Name { get; }
        int Priority { get; }
        ISet<IDeclaration> Declarations { get; }
        IList<IPredicate> Predicates { get; }
        IList<ICondition> Conditions { get; }
        IList<IRuleAction> Actions { get; }
    }

    internal class CompiledRule : ICompiledRule
    {
        public static int DefaultPriority
        {
            get { return 0; }
        }

        public CompiledRule()
        {
            Name = string.Empty;
            Priority = DefaultPriority;
            Handle = Guid.NewGuid().ToString();
            Declarations = new HashSet<IDeclaration>();
            Predicates = new List<IPredicate>();
            Conditions = new List<ICondition>();
            Actions = new List<IRuleAction>();
        }

        public string Handle { get; private set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public ISet<IDeclaration> Declarations { get; private set; }
        public IList<IPredicate> Predicates { get; private set; }
        public IList<ICondition> Conditions { get; private set; }
        public IList<IRuleAction> Actions { get; private set; }
    }
}