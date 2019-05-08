using System.Linq;

namespace NRules.RuleModel
{
    public static class RuleSetExtensions
    {
        /// <summary>
        /// Adds a rule to the rule set.
        /// </summary>
        /// <param name="ruleSet">Rule set instance.</param>
        /// <param name="ruleDefinition">Rule definition to add.</param>
        public static void Add(this IRuleSet ruleSet, IRuleDefinition ruleDefinition)
        {
            ruleSet.Add(Enumerable.Repeat(ruleDefinition, 1));
        }
    }
}