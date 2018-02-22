using System;

namespace NRules.RuleModel
{
    /// <summary>
    /// Fact in the engine's working memory.
    /// </summary>
    public interface IFact
    {
        /// <summary>
        /// Fact runtime type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Fact value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Source of this fact, for synthetic facts, or <c>null</c>.
        /// </summary>
        IFactSource Source { get; }
    }
}