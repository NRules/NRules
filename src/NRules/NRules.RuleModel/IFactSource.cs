using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Type of source that produced the fact.
    /// </summary>
    public enum FactSourceType
    {
        /// <summary>
        /// Fact produced by an aggregation.
        /// </summary>
        Aggregate = 1,

        /// <summary>
        /// Fact produced as a linked fact from a rule action.
        /// </summary>
        Linked = 2,
    }

    /// <summary>
    /// Source of the fact, for synthetic facts.
    /// </summary>
    public interface IFactSource
    {
        /// <summary>
        /// Type of source that produced this fact.
        /// </summary>
        FactSourceType SourceType { get; }

        /// <summary>
        /// Facts that produced this fact.
        /// </summary>
        IEnumerable<IFact> Facts { get; }
    }
}