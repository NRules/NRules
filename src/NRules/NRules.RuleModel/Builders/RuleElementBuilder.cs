using System.Collections.Generic;

namespace NRules.RuleModel.Builders
{
    /// <summary>
    /// Base class for rule element builders.
    /// </summary>
    public abstract class RuleElementBuilder
    {
        internal SymbolTable Scope { get; }

        internal RuleElementBuilder(SymbolTable scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// Pattern declarations visible by the element being built.
        /// </summary>
        public IEnumerable<Declaration> Declarations => Scope.VisibleDeclarations;
    }
}