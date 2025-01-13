using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions;

internal interface ISymbolLookup
{
    bool TryGetValue(string name, out Declaration declaration);
}

internal class SymbolTable : ISymbolLookup
{
    private readonly Dictionary<string, Declaration> _declarations;
    private readonly SymbolTable? _parentScope;

    internal SymbolTable()
    {
        _declarations = new Dictionary<string, Declaration>();
    }

    internal SymbolTable(SymbolTable parentScope)
        : this()
    {
        _parentScope = parentScope;
    }

    public void Add(Declaration declaration)
    {
        _declarations[declaration.Name] = declaration;
    }

    public bool TryGetValue(string name, out Declaration declaration)
    {
        return _declarations.TryGetValue(name, out declaration) || _parentScope?.TryGetValue(name, out declaration) == true;
    }
}