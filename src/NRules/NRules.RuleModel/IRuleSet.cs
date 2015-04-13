using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Represents a named set of rules.
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

    /// <summary>
    /// Default implementation of a rule set.
    /// </summary>
    public class RuleSet : IRuleSet
    {
        private readonly List<IRuleDefinition> _rules = new List<IRuleDefinition>();

        public RuleSet(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public void Add(IEnumerable<IRuleDefinition> ruleDefinitions)
        {
            _rules.AddRange(ruleDefinitions);
        }

        public IEnumerable<IRuleDefinition> Rules
        {
            get { return _rules; }
        }
    }
}