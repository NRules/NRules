using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Rules repository based on the rules defined inline using internal DSL.
    /// </summary>
    public class RuleRepository : IRuleRepository
    {
        private const string DefaultRuleSetName = "default";
        private readonly IList<IRuleSet> _ruleSets = new List<IRuleSet>();

        public RuleRepository()
        {
            Activator = new RuleActivator();
        }

        /// <summary>
        /// Rules activator that instantiates rules based on a .NET type.
        /// </summary>
        public IRuleActivator Activator { get; set; }

        public IEnumerable<IRuleSet> GetRuleSets()
        {
            return _ruleSets;
        }

        /// <summary>
        /// Loads rules into a rule set using provided loader specification.
        /// </summary>
        /// <param name="specAction">Rule loader specification.</param>
        public void Load(Action<IRuleLoadSpec> specAction)
        {
            var spec = new RuleLoadSpec();
            specAction(spec);

            var rules = spec.Load()
                .Select(t => Activator.Activate(t))
                .Select(r => r.GetDefinition());

            var ruleSetName = spec.RuleSetName ?? DefaultRuleSetName;
            var ruleSet = GetRuleSet(ruleSetName);
            ruleSet.Add(rules);

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