using System.Collections.Generic;

namespace NRules.Rule.Builders
{
    internal interface IBuilder<out T>
    {
        T Build();
    }

    public abstract class RuleElementBuilder
    {
        internal SymbolTable Scope { get; private set; }

        internal RuleElementBuilder(SymbolTable scope)
        {
            Scope = scope;
        }

        public IEnumerable<Declaration> Declarations
        {
            get { return Scope.Declarations; }
        }
    }
}