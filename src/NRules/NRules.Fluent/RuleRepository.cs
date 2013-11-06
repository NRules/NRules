using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public IEnumerable<IRuleDefinition> GetRules()
        {
            return _ruleSets.SelectMany(rs => rs.Rules, (rs, r) => r);
        }

        /// <summary>
        /// Finds rules internal DSL rules in a .NET assembly and loads them into the repository.
        /// </summary>
        /// <param name="assembly">Assembly to load rules from.</param>
        public void AddFromAssembly(Assembly assembly)
        {
            Type[] ruleTypes = assembly.GetTypes().Where(IsRule).ToArray();

            if (!ruleTypes.Any())
            {
                throw new ArgumentException(string.Format(
                    "The supplied assembly does not contain any concrete fluent rule definitions. Assembly={0}",
                    assembly.FullName));
            }

            AddFromTypes(ruleTypes);
        }

        /// <summary>
        /// Loads internal DSL rules from .NET types into the repository.
        /// </summary>
        /// <param name="ruleTypes">List of rule types.</param>
        public void AddFromTypes(params Type[] ruleTypes)
        {
            Type[] invalidTypes = ruleTypes.Where(IsNotRule).ToArray();

            if (invalidTypes.Any())
            {
                string invalidTypesString = String.Join(", ", (string[]) invalidTypes.Select(t => t.FullName));
                throw new ArgumentException(string.Format(
                    "The supplied types are not recognized as valid rules. Types={0}",
                    invalidTypesString));
            }

            var ruleSet = new RuleSet();
            AddRulesToRuleSet(ruleTypes, ruleSet);

            AddRuleSet(ruleSet);
        }

        /// <summary>
        /// Adds an existing ruleset to the repository.
        /// </summary>
        /// <param name="ruleSet">Ruleset to add.</param>
        public void AddRuleSet(IRuleSet ruleSet)
        {
            _ruleSets.Add(ruleSet);
        }

        private void AddRulesToRuleSet(Type[] types, IRuleSet ruleSet)
        {
            foreach (Type type in types)
            {
                Dsl.Rule instance = Activator.Activate(type);
                instance.Define();
                IRuleDefinition rule = instance.Builder.Build();
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
                typeof(Dsl.Rule).IsAssignableFrom(type)) return true;

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
    }
}