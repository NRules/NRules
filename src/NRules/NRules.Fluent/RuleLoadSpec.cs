using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.Fluent.Dsl;

namespace NRules.Fluent
{
    /// <summary>
    /// Fluent specification to load rule definitions via reflection.
    /// </summary>
    public interface IRuleLoadSpec
    {
        /// <summary>
        /// Specify to load all rule definitions from a given collection of assemblies.
        /// </summary>
        /// <param name="assemblies">Assemblies to load from.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec From(params Assembly[] assemblies);

        /// <summary>
        /// Specify to load rule definitions from a given collection of types.
        /// </summary>
        /// <param name="types">Types that represent rule definitions.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec From(params Type[] types);

        /// <summary>
        /// Specify which rules to load by filtering on rule's metadata.
        /// </summary>
        /// <param name="filter">Filter condition based on rule's metadata.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec Where(Func<IRuleMetadata, bool> filter);

        /// <summary>
        /// Specify the name of the rule set where to load the rules to.
        /// If not provided, loads rules into default rule set.
        /// </summary>
        /// <param name="ruleSetName">Name of the rule set to load rules to.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec To(string ruleSetName);
    }

    internal class RuleLoadSpec : IRuleLoadSpec
    {
        private readonly List<Type> _ruleTypes = new List<Type>();
        private Func<IRuleMetadata, bool> _filter;

        public string RuleSetName { get; set; }

        public IRuleLoadSpec From(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var ruleTypes = assembly.GetTypes().Where(IsRule).ToArray();
                if (!ruleTypes.Any())
                {
                    throw new ArgumentException(string.Format(
                        "The supplied assembly does not contain any concrete fluent rule definitions. Assembly={0}",
                        assembly.FullName));
                }
                _ruleTypes.AddRange(ruleTypes);
            }
            return this;
        }

        public IRuleLoadSpec From(params Type[] types)
        {
            var invalidTypes = types.Where(IsNotRule).ToArray();
            if (invalidTypes.Any())
            {
                string invalidTypesString = string.Join(", ", invalidTypes.Select(t => t.FullName).ToArray());
                throw new ArgumentException(string.Format(
                    "The supplied types are not recognized as valid rule definitions. Types={0}",
                    invalidTypesString));
            }
            _ruleTypes.AddRange(types);
            return this;
        }

        public IRuleLoadSpec Where(Func<IRuleMetadata, bool> filter)
        {
            if (_filter != null)
            {
                throw new InvalidOperationException("Rule load specification can only have a single 'Where' clause");
            }
            _filter = filter;
            return this;
        }

        public IRuleLoadSpec To(string ruleSetName)
        {
            if (RuleSetName != null)
            {
                throw new InvalidOperationException("Rule load specification can only have a single 'To' clause");
            }
            RuleSetName = ruleSetName;
            return this;
        }

        public IEnumerable<Type> Load()
        {
            var ruleTypes = _ruleTypes;
            if (IsFilterSet())
            {
                var metadata = _ruleTypes.Select(ruleType => new RuleMetadata(ruleType));
                var filteredTypes = metadata.Where(x => _filter(x)).Select(x => x.RuleType);
                ruleTypes = filteredTypes.ToList();
            }
            return ruleTypes;
        }

        private bool IsFilterSet()
        {
            return _filter != null;
        }

        private static bool IsNotRule(Type type)
        {
            return !IsRule(type);
        }

        private static bool IsRule(Type type)
        {
            if (IsPublicConcrete(type) &&
                typeof(Rule).IsAssignableFrom(type)) return true;

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
