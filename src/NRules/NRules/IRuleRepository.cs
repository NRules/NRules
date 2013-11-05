using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules
{
    /// <summary>
    /// In-memory database of production rules.
    /// </summary>
    public interface IRuleRepository
    {
        /// <summary>
        /// Retrieves all rules contained in the repository.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRuleDefinition> GetRules();
    }
}