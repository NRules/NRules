using System;

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
}