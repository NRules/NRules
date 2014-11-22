using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// In-memory database of production rules arranged into rule sets.
    /// <seealso cref="IRuleSet"/>
    /// </summary>
    public interface IRuleRepository
    {
        /// <summary>
        /// Retrieves all rule sets contained in the repository.
        /// </summary>
        /// <returns>Collection of rule sets.</returns>
        IEnumerable<IRuleSet> GetRuleSets();
    }
}