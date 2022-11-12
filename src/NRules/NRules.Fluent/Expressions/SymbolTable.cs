using NRules.RuleModel;

namespace NRules.Fluent.Expressions;

internal class SymbolTable
{
    private readonly HashSet<Declaration> _declarations = new();
    private readonly SymbolTable? _parentScope;

    internal SymbolTable(SymbolTable? parentScope = null)
    {
        _parentScope = parentScope;
    }

    public IEnumerable<Declaration> Declarations => _parentScope?.Declarations.Union(_declarations) ?? _declarations;

    public void Add(Declaration declaration)
    {
        _declarations.Add(declaration);
    }
}