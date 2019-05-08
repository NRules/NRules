using System;
using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Creates instances of <see cref="IRuleDefinition"/> from the fluent DSL <see cref="Rule"/> instances.
    /// </summary>
    public class RuleDefinitionFactory
    {
        /// <summary>
        /// Creates instances of <see cref="IRuleDefinition"/> from the fluent DSL <see cref="Rule"/> instances.
        /// </summary>
        /// <param name="rules">Fluent DSL <see cref="Rule"/> instances.</param>
        /// <returns>Corresponding instances of <see cref="IRuleDefinition"/>.</returns>
        public IEnumerable<IRuleDefinition> Create(IEnumerable<Rule> rules)
        {
            var ruleDefinitions = new List<IRuleDefinition>();
            foreach (var rule in rules)
            {
                var ruleDefinition = Create(rule);
                ruleDefinitions.Add(ruleDefinition);
            }

            return ruleDefinitions;
        }

        /// <summary>
        /// Creates a <see cref="IRuleDefinition"/> for an instance of a fluent DSL <see cref="Rule"/>.
        /// </summary>
        /// <param name="rule">Fluent DSL <see cref="Rule"/> instance.</param>
        /// <returns>Corresponding instance of <see cref="IRuleDefinition"/>.</returns>
        public IRuleDefinition Create(Rule rule)
        {
            try
            {
                return rule.GetDefinition();
            }
            catch (Exception e)
            {
                throw new RuleDefinitionException("Failed to build rule definition", rule.GetType(), e);
            }
        }
    }
}
