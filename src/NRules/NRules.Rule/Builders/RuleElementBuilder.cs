using System.Collections.Generic;

namespace NRules.Rule.Builders
{
    internal interface IBuilder<out T>
    {
        T Build();
    }

    /// <summary>
    /// Base class for rule element builders.
    /// </summary>
    public abstract class RuleElementBuilder
    {
        internal SymbolTable Scope { get; private set; }

        internal RuleElementBuilder(SymbolTable scope)
        {
            Scope = scope;
        }

        /// <summary>
        /// Pattern declarations visible by the element being built.
        /// </summary>
        public IEnumerable<Declaration> Declarations
        {
            get { return Scope.Declarations; }
        }
    }
}