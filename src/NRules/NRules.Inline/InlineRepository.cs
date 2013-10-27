using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Dsl;
using NRules.Rule;
using NRules.Rule.Builders;

namespace NRules.Inline
{
    /// <summary>
    /// Rules repository based on the rules defined inline in a .NET assembly using internal DSL.
    /// </summary>
    public interface IInlineRepository : IRuleRepository
    {
        /// <summary>
        /// Rules activator that instantiates rules based on a .NET type.
        /// </summary>
        IRuleActivator Activator { get; set; }

        /// <summary>
        /// Finds rules internal DSL rules in a .NET assembly and loads them into the repository.
        /// </summary>
        /// <param name="assembly">Assembly to load rules from.</param>
        void AddFromAssembly(Assembly assembly);

        /// <summary>
        /// Loads internal DSL rules from .NET types into the repository.
        /// </summary>
        /// <param name="ruleTypes">List of rule types.</param>
        void AddFromTypes(params Type[] ruleTypes);

        /// <summary>
        /// Adds an existing ruleset to the repository.
        /// </summary>
        /// <param name="ruleSet">Ruleset to add.</param>
        void AddRuleSet(IRuleSet ruleSet);
    }

    /// <summary>
    /// Rules repository based on the rules defined inline in a .NET assembly using internal DSL.
    /// </summary>
    public class InlineRepository : RuleRepository, IInlineRepository
    {
        private readonly IList<IRuleSet> _ruleSets = new List<IRuleSet>();

        public InlineRepository()
        {
            Activator = new RuleActivator();
        }

        public IRuleActivator Activator { get; set; }

        public void AddFromAssembly(Assembly assembly)
        {
            Type[] ruleTypes = assembly.GetTypes().Where(IsRule).ToArray();

            if (!ruleTypes.Any())
            {
                throw new ArgumentException(string.Format(
                    "The supplied assembly does not contain any concrete IRule implementations. Assembly={0}",
                    assembly.FullName));
            }

            AddFromTypes(ruleTypes);
        }

        public void AddFromTypes(params Type[] types)
        {
            Type[] invalidTypes = types.Where(IsNotRule).ToArray();

            if (invalidTypes.Any())
            {
                string invalidTypesString = String.Join(", ", (string[]) invalidTypes.Select(t => t.FullName));
                throw new ArgumentException(string.Format(
                    "The supplied types are not recognized as valid rules. Types={0}",
                    invalidTypesString));
            }

            var ruleSet = new RuleSet();
            AddRulesToRuleSet(types, ruleSet);

            AddRuleSet(ruleSet);
        }

        public void AddRuleSet(IRuleSet ruleSet)
        {
            _ruleSets.Add(ruleSet);
        }

        private void AddRulesToRuleSet(Type[] types, IRuleSet ruleSet)
        {
            foreach (Type type in types)
            {
                IRule instance = Activator.Activate(type);
                var builder = new RuleBuilder();
                var definition = new Definition(builder, instance);

                builder.Name(instance.GetType().FullName);
                instance.Define(definition);

                IRuleDefinition rule = builder.Build();
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