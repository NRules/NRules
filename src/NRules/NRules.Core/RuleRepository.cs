using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Config;
using NRules.Dsl;
using NRules.Rule;

namespace NRules.Core
{
    public interface IRuleBase
    {
        IEnumerable<IRuleDefinition> Rules { get; }
    }

    public interface IRuleRepository : IRuleBase
    {
        IRuleSet AddRuleSet(Assembly assembly);
        IRuleSet AddRuleSet(params Type[] ruleTypes);
        ISessionFactory CreateSessionFactory();
    }

    internal class RuleRepository : IRuleRepository
    {
        private readonly IList<IRuleSet> _ruleSets = new List<IRuleSet>();
        public IContainer Container { get; set; }

        public IRuleSet AddRuleSet(Assembly assembly)
        {
            var ruleTypes = assembly.GetTypes().Where(IsRule).ToArray();

            if (!ruleTypes.Any())
            {
                throw new ArgumentException(string.Format(
                    "The supplied assembly does not contain any concrete IRule implementations. Assembly={0}",
                    assembly.FullName));
            }

            var ruleSet = AddRuleSet(ruleTypes);
            return ruleSet;
        }

        public ISessionFactory CreateSessionFactory()
        {
            return Container.Build<ISessionFactory>();
        }

        public IRuleSet AddRuleSet(params Type[] types)
        {
            var invalidTypes = types.Where(IsNotRule).ToArray();

            if (invalidTypes.Any())
            {
                var invalidTypesString = String.Join(", ", (string[]) invalidTypes.Select(t => t.FullName));
                throw new ArgumentException(string.Format(
                    "The supplied types are not recognized as valid rules. Types={0}",
                    invalidTypesString));
            }

            var ruleSet = new RuleSet();
            _ruleSets.Add(ruleSet);

            AddRulesToRuleSet(types, ruleSet);

            return ruleSet;
        }

        private void AddRulesToRuleSet(Type[] types, IRuleSet ruleSet)
        {
            IEnumerable<IRule> ruleInstances;
            using (var container = Container.CreateChildContainer())
            {
                Array.ForEach(types, t => Container.Configure(t, DependencyLifecycle.InstancePerCall));
                ruleInstances = container.BuildAll<IRule>();
            }

            foreach (var ruleInstance in ruleInstances)
            {
                var metadata = new RuleMetadata(ruleInstance);
                var builder = ruleSet.AddRule();
                builder.Name(ruleInstance.GetType().FullName);
                var definition = new Definition(builder, metadata);
                ruleInstance.Define(definition);
            }
        }

        private static bool IsNotRule(Type type)
        {
            return !IsRule(type);
        }

        private static bool IsRule(Type type)
        {
            if (IsPublicConcrete(type) &&
                typeof (IRule).IsAssignableFrom(type)) return true;

            return false;
        }

        private static bool IsPublicConcrete(Type type)
        {
            if (!type.IsPublic) return false;
            if (type.IsAbstract) return false;
            if (type.IsInterface) return false;
            if (type.IsGenericTypeDefinition) return false;

            return true;
        }

        public IEnumerable<IRuleDefinition> Rules
        {
            get { return _ruleSets.SelectMany(rs => rs.Rules, (rs, r) => r); }
        }
    }
}