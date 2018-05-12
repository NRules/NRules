using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements on the right-hand side of the rule definition.
    /// </summary>
    public abstract class RuleRightElement : RuleElement
    {
        internal RuleRightElement(IEnumerable<Declaration> declarations) : base(declarations) { }
    }
}