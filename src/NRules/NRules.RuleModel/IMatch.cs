using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Event that triggered the match.
    /// </summary>
    public enum MatchTrigger
    {
        /// <summary>
        /// Match is not active.
        /// </summary>
        None = 0,

        /// <summary>
        /// Match is triggered due to activation creation.
        /// </summary>
        Created = 1,

        /// <summary>
        /// Match is triggered due to activation update.
        /// </summary>
        Updated = 2,

        /// <summary>
        /// Match is triggered due to activation removal.
        /// </summary>
        Removed = 4,
    }

    /// <summary>
    /// Represents a match of all rule's conditions.
    /// </summary>
    public interface IMatch
    {
        /// <summary>
        /// Rule that matched the given facts.
        /// </summary>
        IRuleDefinition Rule { get; }

        /// <summary>
        /// Facts matched by the rule.
        /// </summary>
        IEnumerable<IFactMatch> Facts { get; }

        /// <summary>
        /// Event that triggered the match.
        /// </summary>
        MatchTrigger Trigger { get; }
    }
}