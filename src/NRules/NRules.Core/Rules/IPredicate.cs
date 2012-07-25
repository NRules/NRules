using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal enum PredicateTypes
    {
        Selection = 0,
        Join = 1,
        Aggregate = 2,
        Existential = 3,
    }

    internal interface IPredicate
    {
        PredicateTypes PredicateType { get; }
        IList<IDeclaration> Declarations { get; }
        IList<ICondition> Conditions { get; }
        IAggregate AggregationStrategy { get; }
    }
}