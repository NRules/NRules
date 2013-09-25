using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Rule
{
    internal class SymbolTable
    {
        private readonly HashSet<Declaration> _symbolTable = new HashSet<Declaration>();

        public SymbolTable ParentScope { get; internal set; }

        public IEnumerable<Declaration> LocalDeclarations
        {
            get { return _symbolTable; }
        }

        public IEnumerable<Declaration> ExternalDeclarations
        {
            get { return (ParentScope == null) ? new Declaration[] {} : ParentScope.Declarations; }
        }

        public IEnumerable<Declaration> Declarations
        {
            get { return LocalDeclarations.Union(ExternalDeclarations); }
        }

        public void Add(Declaration declaration)
        {
           _symbolTable.Add(declaration);
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