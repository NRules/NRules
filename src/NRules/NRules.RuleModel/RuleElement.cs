using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements.
    /// </summary>
    public abstract class RuleElement
    {
        public abstract IEnumerable<Declaration> Declarations { get; } 
        internal RuleElement() { }
        internal abstract void Accept<TContext>(TContext context, RuleElementVisitor<TContext> visitor);
    }
}