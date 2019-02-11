using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal class SymbolTable
    {
        private readonly HashSet<Declaration> _declarations;
        private readonly SymbolTable _parentScope;

        internal SymbolTable()
        {
            _declarations = new HashSet<Declaration>();
        }

        internal SymbolTable(SymbolTable parentScope)
            : this()
        {
            _parentScope = parentScope;
        }

        public IEnumerable<Declaration> Declarations => _parentScope?.Declarations.Union(_declarations) ?? _declarations;

        public void Add(Declaration declaration)
        {
            _declarations.Add(declaration);
        }
    }
}