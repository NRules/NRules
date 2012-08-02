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
        private readonly IContainer _container;
        private readonly Func<Type, IRule> _ruleFactory;

        public RuleRepository()
        {
            _ruleFactory = type => (IRule) Activator.CreateInstance(type);
        }

        public RuleRepository(IContainer container)
        {
            _container = container;
            _ruleFactory = type => (IRule) _container.GetObjectInstance(type);
        }

        public void AddRuleSet(Assembly assembly)
        {
            var ruleTypes = assembly.GetTypes().Where(IsRule).ToArray();

            if (!ruleTypes.Any())
            {
                throw new ArgumentException(string.Format(
                    "The supplied assembly does not contain any concrete IRule implementations. Assembly={0}",
                    assembly.FullName));
            }

            var ruleSet = new RuleSet(ruleTypes);
            _ruleSets.Add(ruleSet);
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

            var ruleSet = new RuleSet(types);
            _ruleSets.Add(ruleSet);
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
                ruleInstance = _ruleFactory.Invoke(ruleType);
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