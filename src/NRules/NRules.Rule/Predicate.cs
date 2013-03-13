using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public enum PredicateTypes
    {
        Selection = 0,
        Aggregate = 1,
        Existential = 2,
    }

    public interface IPredicate
    {
        PredicateTypes PredicateType { get; }
        IDeclaration Declaration { get; }
        Type StrategyType { get; set; }
    }

    internal class Predicate : IPredicate
    {
        public Predicate(PredicateTypes predicateType, IDeclaration declaration)
        {
            PredicateType = predicateType;
            Declaration = declaration;
            Conditions = new List<ICondition>();
        }

        public PredicateTypes PredicateType { get; private set; }
        public IDeclaration Declaration { get; private set; }
        public IList<ICondition> Conditions { get; private set; }
        public Type StrategyType { get; set; }
    }
}