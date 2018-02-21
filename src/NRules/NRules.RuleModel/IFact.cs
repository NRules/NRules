using System;
using System.Collections.Generic;

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
        /// Source facts that produced this fact (for synthetic facts) or <c>null</c>.
        /// </summary>
        IEnumerable<IFact> Source { get; }
    }
}