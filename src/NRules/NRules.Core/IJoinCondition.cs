using System;
using System.Collections.Generic;
using NRules.Core.Rete;

namespace NRules.Core
{
    internal interface IJoinCondition
    {
        string Key { get; }
        IEnumerable<Type> FactTypes { get; }
        bool IsSatisfiedBy(params Fact[] facts);
    }
}