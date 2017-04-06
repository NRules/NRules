using System.Collections.Generic;

namespace NRules
{
    /// <summary>
    /// Result of an operation on a set of facts.
    /// </summary>
    public interface IFactResult
    {
        /// <summary>
        /// Number of facts on which the operation failed.
        /// </summary>
        int FailedCount { get; }

        /// <summary>
        /// Facts on which the operation failed.
        /// </summary>
        IEnumerable<object> Failed { get; }
    }
}