using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Rules repository based on the rules defined inline in a .NET assembly using internal DSL.
    /// </summary>
    public class RuleRepository : IRuleRepository
    {
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
        /// <param name="ruleSetName">Name of the rule set.</param>
        /// <param name="specAction">Rule loader specification.</param>
        public void Load(string ruleSetName, Action<IRuleLoadSpec> specAction)
        {
            var spec = new RuleLoadSpec();
            specAction(spec);

            var rules = spec.Load()
                .Select(t => Activator.Activate(t))
                .Select(r => r.GetDefinition());

            var ruleSet = new RuleSet(ruleSetName);
            ruleSet.Add(rules);

            _ruleSets.Add(ruleSet);
        }
    }
}