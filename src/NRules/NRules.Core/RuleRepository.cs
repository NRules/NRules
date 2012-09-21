using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Config;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface IRuleRepository
    {
        void AddRuleSet(Assembly assembly);
        ISessionFactory CreateSessionFactory();
    }

    internal class RuleRepository : IRuleRepository
    {
        private readonly IList<RuleSet> _ruleSets = new List<RuleSet>();
        public IContainer Container { get; set; }

        public void AddRuleSet(Assembly assembly)
        {
            var ruleTypes = assembly.GetTypes().Where(IsRule).ToArray();

            if (!ruleTypes.Any())
            {
                throw new ArgumentException(string.Format(
                    "The supplied assembly does not contain any concrete IRule implementations. Assembly={0}",
                    assembly.FullName));
            }

            WireRulesWithContainer(ruleTypes);
            var ruleSet = new RuleSet(ruleTypes);
            _ruleSets.Add(ruleSet);
        }

        public ISessionFactory CreateSessionFactory()
        {
            return Container.Build<ISessionFactory>();
        }

        public void AddRuleSet(params Type[] types)
        {
            var invalidTypes = types.Where(IsNotRule).ToArray();

            if (invalidTypes.Any())
            {
                var invalidTypesString = String.Join(", ", (string[]) invalidTypes.Select(t => t.FullName));
                throw new ArgumentException(string.Format(
                    "The supplied types are not recognized as valid rules. Types={0}",
                    invalidTypesString));
            }

            WireRulesWithContainer(types);
            var ruleSet = new RuleSet(types);
            _ruleSets.Add(ruleSet);
        }

        private void WireRulesWithContainer(Type[] ruleTypes)
        {
            Array.ForEach(ruleTypes, t => Container.Configure(t, DependencyLifecycle.InstancePerCall));
        }

        internal IEnumerable<CompiledRule> Compile()
        {
            if (!_ruleSets.Any())
            {
                throw new ArgumentException("No valid rulesets in the repository");
            }

            foreach (RuleSet ruleSet in _ruleSets)
            {
                foreach (Type ruleType in ruleSet.RuleTypes)
                {
                    CompiledRule rule = InstantiateRule(ruleType);
                    yield return rule;
                }
            }
        }

        private CompiledRule InstantiateRule(Type ruleType)
        {
            IRule ruleInstance;

            try
            {
                ruleInstance = (IRule) Container.Build(ruleType);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format("Failed to instantiate a rule. Rule Type={0}", ruleType), e);
            }

            var rule = new CompiledRule(ruleType.FullName);
            var definition = new RuleDefinition(rule);

            ruleInstance.Define(definition);
            return rule;
        }

        private static bool IsNotRule(Type type)
        {
            return !IsRule(type);
        }

        private static bool IsRule(Type type)
        {
            if (IsConcrete(type) &&
                typeof (IRule).IsAssignableFrom(type)) return true;

            return false;
        }

        private static bool IsConcrete(Type type)
        {
            if (type.IsAbstract) return false;
            if (type.IsInterface) return false;
            if (type.IsGenericTypeDefinition) return false;

            return true;
        }
    }
}