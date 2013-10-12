using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Rule
{
    internal class SymbolTable
    {
        private readonly HashSet<Declaration> _symbolTable = new HashSet<Declaration>();

        public SymbolTable ParentScope { get; internal set; }

        //todo: refactor this
        public SymbolTable()
        {
        }

        public SymbolTable(SymbolTable parentScope)
        {
            ParentScope = parentScope;
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

        public Declaration Declare(string name, Type type)
        {
            string declarationName = name ?? "$local$";
            bool isLocal = (name == null);
            var declaration = new Declaration(declarationName, type, isLocal);
            Add(declaration);
            return declaration;
        }

        public Declaration Lookup(string name, Type type)
        {
            Declaration declaration = _symbolTable.FirstOrDefault(d => d.Name == name);
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
                return ParentScope.Lookup(name, type);
            }
            return null;
        }
    }
}