using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Represents a match of all rule's conditions.
    /// </summary>
    public interface IMatch
    {
        /// <summary>
        /// Rule that got activated.
        /// </summary>
        IRuleDefinition Rule { get; }

        /// <summary>
        /// Facts matched by the rule.
        /// </summary>
        IEnumerable<IFactMatch> Facts { get; }
    }
}