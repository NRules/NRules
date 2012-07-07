using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Core.Rules;

namespace NRules.Core
{
    public class RuleRepository
    {
        private readonly IList<RuleSet> _ruleSets = new List<RuleSet>();

        public void AddRuleSet(Assembly assembly)
        {
            var ruleTypes = assembly.GetTypes()
                .Where(IsRule);
            var ruleSet = new RuleSet(ruleTypes);
            _ruleSets.Add(ruleSet);
        }

        internal IEnumerable<Rule> Compile()
        {
            foreach (var ruleSet in _ruleSets)
            {
                foreach (var ruleType in ruleSet.RuleTypes)
                {
                    IRule ruleInstance = BuildRule(ruleType);
                    var rule = new Rule(ruleInstance.GetType().FullName);
                    var definition = new RuleDefinition(rule);
                    ruleInstance.Define(definition);
                    yield return rule;
                }
            }
        }

        private bool IsRule(Type type)
        {
            if (IsConcrete(type) &&
                typeof (IRule).IsAssignableFrom(type)) return true;

            return false;
        }

        private bool IsConcrete(Type type)
        {
            if (type.IsAbstract) return false;
            if (type.IsInterface) return false;
            if (type.IsGenericTypeDefinition) return false;

            return true;
        }

        private IRule BuildRule(Type type)
        {
            var rule = (IRule) Activator.CreateInstance(type);
            return rule;
        }
    }
}