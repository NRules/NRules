using System;
using System.Collections.Generic;
using NRules.Core.Rete;

namespace NRules.Core.Rules
{
    internal interface ICondition
    {
        IEnumerable<Type> FactTypes { get; }
        bool IsSatisfiedBy(params Fact[] facts);
    }
}