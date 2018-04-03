using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Set of facts matched by the rules engine.
    /// </summary>
    /// <seealso cref="IFact"/>
    public interface ITuple
    {
        /// <summary>
        /// Facts in the tuple, representing a partial match in the engine's working memory.
        /// </summary>
        /// <remarks>Facts in the tuple are stored in the reverse order.</remarks>
        IEnumerable<IFact> Facts { get; }

        /// <summary>
        /// Number of facts in the tuple.
        /// </summary>
        int Count { get; }
    }
}
