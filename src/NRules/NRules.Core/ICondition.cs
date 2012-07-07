using System;
using NRules.Core.Rete;

namespace NRules.Core
{
    internal interface ICondition
    {
        string Key { get; }
        Type FactType { get; }
        bool IsSatisfiedBy(Fact fact);
    }
}