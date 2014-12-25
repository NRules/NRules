using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Base class for rule elements on the left hand side of the rule definition.
    /// </summary>
    public abstract class RuleLeftElement : RuleElement
    {
        internal RuleLeftElement(IEnumerable<Declaration> declarations) : base(declarations) { }
    }
}