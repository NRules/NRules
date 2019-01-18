using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Fluent.Expressions
{
    internal class SymbolTable
    {
        private readonly List<Declaration> _declarations;
        private readonly SymbolTable _parentScope;

        internal SymbolTable()
        {
            _declarations = new List<Declaration>();
        }

        internal SymbolTable(SymbolTable parentScope)
            : this()
        {
            _parentScope = parentScope;
        }

        public IEnumerable<Declaration> Declarations => _parentScope?.Declarations.Concat(_declarations) ?? _declarations;

        public void Add(Declaration declaration)
        {
            _declarations.Add(declaration);
        }
    }
}