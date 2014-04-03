using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Represents a set of rules
    /// </summary>
    public interface IRuleSet
    {
        /// <summary>
        /// Rule set name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Adds rules to the ruleset.
        /// </summary>
        /// <param name="ruleDefinitions">Rule definitions to add.</param>
        void Add(IEnumerable<IRuleDefinition> ruleDefinitions);

        /// <summary>
        /// Rules in the ruleset.
        /// </summary>
        IEnumerable<IRuleDefinition> Rules { get; }
    }
}