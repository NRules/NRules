using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface IRuleRepository
    {
        void AddRuleSet(Assembly assembly);
    }

    public class RuleRepository : IRuleRepository
    {
        private readonly IList<RuleSet> _ruleSets = new List<RuleSet>();

        private readonly IContainer _diContainer;

        public RuleRepository()
        {
            _diContainer = null;
        }

        public RuleRepository(IContainer diContainer)
        {
            _diContainer = diContainer;
        }

        public void AddRuleSet(Assembly assembly)
        {
            IEnumerable<Type> ruleTypes = assembly.GetTypes().Where(IsRule);

            if (!ruleTypes.Any())
                throw new ArgumentException(string.Format("The supplied assembly ({0}) does not contain " +
                                                          "any concrete IRule implementations!",
                                                          assembly.FullName));

            var ruleSet = new RuleSet(ruleTypes);
            _ruleSets.Add(ruleSet);
        }

        internal IEnumerable<Rule> Compile()
        {
            if (!_ruleSets.Any())
                throw new ArgumentException("Rules cannot be compiled! No valid rulesets have been added to the rule repository.");

            foreach (RuleSet ruleSet in _ruleSets)
            {
                foreach (Type ruleType in ruleSet.RuleTypes)
                {
                    Rule rule = InstantiateRule(ruleType);
                    yield return rule;
                }
            }
        }

        private Rule InstantiateRule(Type ruleType)
        {
            IRule ruleInstance;

            if (_diContainer == null)
            {
                ruleInstance = BuildRule(ruleType);
            }
            else
            {
                object objectInstance = _diContainer.GetObjectInstance(ruleType);
                ruleInstance = objectInstance as IRule;
                if (ruleInstance == null)
                    throw new ApplicationException(string.Format("Failed to initialize rule of type {0} from dependency injection " +
                                                                 "container of type {1}.",
                                                                 ruleType,
                                                                 _diContainer.GetType()));
            }

            var rule = new Rule(ruleInstance.GetType().FullName);
            var definition = new RuleDefinition(rule);

            ruleInstance.Define(definition);
            return rule;
        }

        private static bool IsRule(Type type)
        {
            if (IsConcrete(type) &&
                typeof(IRule).IsAssignableFrom(type)) return true;

            return false;
        }

        private static bool IsConcrete(Type type)
        {
            if (type.IsAbstract) return false;
            if (type.IsInterface) return false;
            if (type.IsGenericTypeDefinition) return false;

            return true;
        }

        private static IRule BuildRule(Type type)
        {
            var rule = (IRule)Activator.CreateInstance(type);
            return rule;
        }
    }
}
