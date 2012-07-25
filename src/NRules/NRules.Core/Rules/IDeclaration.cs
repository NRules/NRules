using System;

namespace NRules.Core.Rules
{
    internal interface IDeclaration
    {
        string Name { get; }
        Type Type { get; }
    }
}