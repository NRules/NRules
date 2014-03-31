using System.Collections.Generic;

namespace NRules.RuleModel
{
    /// <summary>
    /// Represents a set of rules
    /// </summary>
    public interface IRuleSet
    {
        /// <summary>
        /// Adds a rule to the ruleset.
        /// </summary>
        /// <param name="ruleDefinition"></param>
        void AddRule(IRuleDefinition ruleDefinition);

        /// <summary>
        /// Rules in the ruleset.
        /// </summary>
        IEnumerable<IRuleDefinition> Rules { get; }
    }

    /// <summary>
    /// Represents a set of rules
    /// </summary>
    public class RuleSet : IRuleSet
    {
        private readonly List<IRuleDefinition> _rules = new List<IRuleDefinition>();

        public RuleSet()
        {
        }

        public RuleSet(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public void AddRule(IRuleDefinition ruleDefinition)
        {
            _rules.Add(ruleDefinition);
        }

        public IEnumerable<IRuleDefinition> Rules
        {
            get { return _rules; }
        }
    }
}