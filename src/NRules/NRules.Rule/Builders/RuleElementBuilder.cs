using System;
using System.Collections.Generic;

namespace NRules.Rule.Builders
{
    public interface INamingScope
    {
        Declaration Declare(string name, Type type);
        Declaration Declare(Type type);
        IEnumerable<Declaration> Declarations { get; }
    }

    internal interface IRuleElementBuilder<out TElement> where TElement: RuleElement
    {
        TElement Build();
    }

    public abstract class RuleElementBuilder : INamingScope
    {
        internal SymbolTable Scope { get; private set; }

        internal RuleElementBuilder(SymbolTable parentScope)
        {
            Scope = new SymbolTable(parentScope);
        }

        protected RuleElementBuilder() : this(null)
        {
        }

        public Declaration Declare(string name, Type type)
        {
            return Scope.Declare(name, type);
        }

        public Declaration Declare(Type type)
        {
            return Declare(null, type);
        }

        public IEnumerable<Declaration> Declarations { get { return Scope.Declarations; } }
    }
}