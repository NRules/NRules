using System;
using System.Collections.Generic;

namespace NRules.Rule
{
    public interface ICondition
    {
        IEnumerable<Type> FactTypes { get; }
        bool IsSatisfiedBy(params object[] factObjects);
    }
}