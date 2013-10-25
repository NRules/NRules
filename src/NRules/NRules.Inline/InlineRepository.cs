using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Core;
using NRules.Dsl;
using NRules.Rule;
using NRules.Rule.Builders;

namespace NRules.Inline
{
    public interface IInlineRepository : IRuleRepository
    {
        IRuleActivator Activator { get; set; }
        IRuleSet AddRuleSet(Assembly assembly);
        IRuleSet AddRuleSet(params Type[] ruleTypes);
    }

    public class InlineRepository : RuleRepository, IInlineRepository
    {
        private readonly IList<IRuleSet> _ruleSets = new List<IRuleSet>();

        public InlineRepository()
        {
            Activator = new RuleActivator();
        }

        public IRuleActivator Activator { get; set; }

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
            foreach (var type in types)
            {
                var instance = Activator.Activate(type);
                var metadata = new RuleMetadata(instance);
                var builder = new RuleBuilder();
                builder.Name(instance.GetType().FullName);
                var definition = new Definition(builder, metadata);
                instance.Define(definition);
                var rule = builder.Build();
                ruleSet.AddRule(rule);
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

        public override IEnumerable<IRuleDefinition> Rules
        {
            get { return _ruleSets.SelectMany(rs => rs.Rules, (rs, r) => r); }
        }
    }
}