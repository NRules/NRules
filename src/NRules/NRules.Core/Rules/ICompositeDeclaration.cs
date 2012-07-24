using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal interface ICompositeDeclaration
    {
        IList<Type> FactTypes { get; }
        IList<ICondition> Conditions { get; }
        IAggregate AggregationStrategy { get; }
    }
}