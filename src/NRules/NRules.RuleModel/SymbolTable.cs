using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel
{
    internal class SymbolTable
    {
        public const string ScopeSeparator = ":";

        private readonly HashSet<Declaration> _symbolTable;
        private readonly string _name;
        private int _declarationCounter = 0;

        public SymbolTable ParentScope { get; private set; }

        internal SymbolTable()
        {
            _symbolTable = new HashSet<Declaration>();
        }

        internal SymbolTable(string name)
        {
            _name = name;
            _symbolTable = new HashSet<Declaration>();
        }

        internal SymbolTable(IEnumerable<Declaration> declarations)
        {
            _symbolTable = new HashSet<Declaration>(declarations);
        }

        internal SymbolTable New(string name)
        {
            var childScope = new SymbolTable(name);
            childScope.ParentScope = this;
            return childScope;
        }

        public string FullName
        {
            get { return (ParentScope == null || ParentScope.FullName == null) ? _name : ParentScope.FullName + ScopeSeparator + _name; }
        }

        public IEnumerable<Declaration> Declarations
        {
            get { return _symbolTable; }
        }

        public IEnumerable<Declaration> VisibleDeclarations
        {
            get { return (ParentScope != null) ? ParentScope.VisibleDeclarations.Concat(Declarations) : Declarations; }
        }

        public void Add(Declaration declaration)
        {
            _symbolTable.Add(declaration);
        }

        public Declaration Declare(Type type, string name)
        {
            _declarationCounter++;
            string declarationName = name ?? string.Format("$var{0}$", _declarationCounter);
            var declaration = new Declaration(type, declarationName, FullName);
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
                    throw new ArgumentException(
                        string.Format("Declaration type mismatch. Name={0}, ExpectedType={1}, FoundType={2}",
                                      name, declaration.Type, type));
                }
                return declaration;
            }
            if (ParentScope != null)
            {
                return ParentScope.Lookup(name, type);
            }

            throw new ArgumentException(string.Format("Declaration not found. Name={0}, Type={1}", name, type));
        }
    }
}