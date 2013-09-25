using System.Collections.Generic;

namespace NRules.Rule
{
    public enum RuleElementTypes
    {
        Match = 0,
        Aggregate = 1,
        Group = 2,
    }

    public abstract class RuleElement
    {
        private readonly SymbolTable _symbolTable = new SymbolTable();

        public IEnumerable<Declaration> Declarations
        {
            get { return SymbolTable.LocalDeclarations; }
        }

        public RuleElementTypes RuleElementType { get; protected set; }

        internal SymbolTable SymbolTable
        {
            get { return _symbolTable; }
        }
    }
}