using System;

namespace NRules.Rule
{
    public interface IDeclaration
    {
        string Name { get; }
        Type Type { get; }
    }
}