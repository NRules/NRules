using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Rule
{
    internal class SymbolTable
    {
        private readonly HashSet<Declaration> _symbolTable = new HashSet<Declaration>();

        public SymbolTable ParentScope { get; private set; }

        internal SymbolTable New()
        {
            var childScope = new SymbolTable();
            childScope.ParentScope = this;
            return childScope;
        }

        public IEnumerable<Declaration> LocalDeclarations
        {
            get { return _symbolTable.Where(d => d.IsLocal); }
        }

        public IEnumerable<Declaration> PublicDeclarations
        {
            get { return _symbolTable.Where(d => !d.IsLocal); }
        }

        public IEnumerable<Declaration> Declarations
        {
            get { return (ParentScope != null) ? ParentScope.Declarations.Union(PublicDeclarations) : PublicDeclarations; }
        }

        public void Add(Declaration declaration)
        {
            _symbolTable.Add(declaration);
        }

        public Declaration Declare(Type type, string name)
        {
            string declarationName = name ?? "$local$";
            bool isLocal = (name == null);
            var declaration = new Declaration(type, declarationName, isLocal);
            Add(declaration);

            if (!declaration.IsLocal && ParentScope != null) ParentScope.Add(declaration);

            return declaration;
        }

        public Declaration Lookup(string name, Type type)
        {
            return Lookup(name, type, includeLocal: true);
        }

        private Declaration Lookup(string name, Type type, bool includeLocal)
        {
            Declaration declaration = includeLocal
                                          ? _symbolTable.FirstOrDefault(d => d.Name == name)
                                          : _symbolTable.FirstOrDefault(d => d.Name == name && !d.IsLocal);

            if (declaration != null)
            {
                if (declaration.Type != type)
                {
                    throw new InvalidOperationException(
                        string.Format("Type mismatch. IdentifierName={0}, ExpectedType={1}, FoundType={2}",
                                      name, declaration.Type, type));
                }
                return declaration;
            }
            if (ParentScope != null)
            {
                return ParentScope.Lookup(name, type, includeLocal: false);
            }

            throw new ArgumentException(string.Format("Declaration not found. Name={0}, Type={1}", name, type));
        }
    }
}