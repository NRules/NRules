using System;
using System.Collections.Generic;

namespace NRules.Core.Rules
{
    internal interface IDeclaration
    {
        string Name { get; }
        Type Type { get; }
        IList<ICondition> Conditions { get; }
    }
}