using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Rules repository based on the rules defined inline using internal DSL.
    /// Use <see cref="Load"/> method to fluently load rules into the repository.
    /// </summary>
    public class RuleRepository : IRuleRepository
    {
        private const string DefaultRuleSetName = "default";
        private readonly List<IRuleSet> _ruleSets = new();

        /// <summary>
        /// Creates an empty rule repository.
        /// </summary>
        public RuleRepository()
        {
            Activator = new RuleActivator();
        }

        /// <summary>
        /// Rules activator that instantiates rules based on a CLR type.
        /// </summary>
        public IRuleActivator Activator { get; set; }

        /// <summary>
        /// Retrieves all rule sets contained in the repository.
        /// </summary>
        /// <returns>Collection of rule sets.</returns>
        public IEnumerable<IRuleSet> GetRuleSets()
        {
            return _ruleSets;
        }

        /// <summary>
        /// Loads rules into a rule set using provided loader specification.
        /// <seealso cref="IRuleLoadSpec"/>
        /// </summary>
        /// <param name="specAction">Rule loader specification.</param>
        public void Load(Action<IRuleLoadSpec> specAction)
        {
            var spec = new RuleLoadSpec(Activator);
            specAction(spec);

            var ruleSetName = spec.RuleSetName ?? DefaultRuleSetName;
            var ruleSet = GetRuleSet(ruleSetName);

            var rules = spec.Load();
            ruleSet.Add(rules);
        }

        /// <summary>
        /// Adds a new rule set to the rule repository.
        /// </summary>
        /// <param name="ruleSet">Rule set to add.</param>
        /// <exception cref="ArgumentException">A rule set with the same name already exists.</exception>
        public void Add(IRuleSet ruleSet)
        {
            if (_ruleSets.Any(x => x.Name == ruleSet.Name))
                throw new ArgumentException($"Rule set with the same name already exists. Name={ruleSet.Name}");

            _ruleSets.Add(ruleSet);
        }

        private IRuleSet GetRuleSet(string ruleSetName)
        {
            IRuleSet ruleSet = _ruleSets.SingleOrDefault(rs => rs.Name == ruleSetName);
            if (ruleSet == null)
            {
                ruleSet = new RuleSet(ruleSetName);
                _ruleSets.Add(ruleSet);
            }
            return ruleSet;
        }
    }
}