using System;

namespace NRules.Core.Rules
{
    internal enum PredicateTypes
    {
        Selection = 0,
        Aggregate = 1,
        Existential = 2,
    }

    internal interface IPredicate
    {
        PredicateTypes PredicateType { get; }
        IDeclaration Declaration { get; }
        Type StrategyType { get; set; }
    }
}