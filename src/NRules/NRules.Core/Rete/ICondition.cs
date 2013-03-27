using System;
using System.Collections.Generic;

namespace NRules.Core.Rete
{
    public interface ICondition
    {
        IEnumerable<Type> FactTypes { get; }
        bool IsSatisfiedBy(params object[] factObjects);
    }
}