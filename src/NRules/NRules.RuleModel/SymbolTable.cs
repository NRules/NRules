using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.RuleModel
{
    internal class SymbolTable
    {
        public const string ScopeSeparator = ":";

        private readonly HashSet<Declaration> _symbolTable;
        private int _declarationCounter = 0;

        public SymbolTable ParentScope { get; }

        internal SymbolTable()
        {
            _symbolTable = new HashSet<Declaration>();
        }

        internal SymbolTable(string name)
        {
            Name = name;
            _symbolTable = new HashSet<Declaration>();
        }

        internal SymbolTable(string name, SymbolTable parentScope)
        {
            Name = name;
            _symbolTable = new HashSet<Declaration>();
            ParentScope = parentScope;
        }

        internal SymbolTable(IEnumerable<Declaration> declarations)
        {
            _symbolTable = new HashSet<Declaration>(declarations);
        }

        internal SymbolTable New(string name)
        {
            var childScope = new SymbolTable(name, this);
            return childScope;
        }

        public string Name { get; }
        public string FullName => ParentScope?.FullName == null ? Name : ParentScope.FullName + ScopeSeparator + Name;
        public IEnumerable<Declaration> Declarations => _symbolTable;
        public IEnumerable<Declaration> VisibleDeclarations => ParentScope?.VisibleDeclarations.Concat(Declarations) ?? Declarations;

        public void Add(Declaration declaration)
        {
            _symbolTable.Add(declaration);
        }

        public Declaration Declare(Type type, string name)
        {
            _declarationCounter++;
            string declarationName = name ?? $"$var{_declarationCounter}$";
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
                        $"Declaration type mismatch. Name={name}, ExpectedType={declaration.Type}, FoundType={type}");
                }
                return declaration;
            }
            if (ParentScope != null)
            {
                return ParentScope.Lookup(name, type);
            }

            throw new ArgumentException($"Declaration not found. Name={name}, Type={type}");
        }
    }
}