using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NRules.RuleModel;

namespace NRules.Fluent
{
    /// <summary>
    /// Fluent specification to load rule definitions via reflection.
    /// </summary>
    public interface IRuleLoadSpec
    {
        /// <summary>
        /// Specifies to load all rule definitions from a given collection of assemblies.
        /// </summary>
        /// <param name="assemblies">Assemblies to load from.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec From(params Assembly[] assemblies);

        /// <summary>
        /// Specifies to load rule definitions from a given collection of types.
        /// </summary>
        /// <param name="types">Types that represent rule definitions.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec From(params Type[] types);

        /// <summary>
        /// Specifies which rules to load by filtering on rule's metadata.
        /// </summary>
        /// <param name="filter">Filter condition based on rule's metadata.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec Where(Func<IRuleMetadata, bool> filter);

        /// <summary>
        /// Specifies the name of the rule set where the rules are loaded to.
        /// If not provided, loads rules into default rule set.
        /// </summary>
        /// <param name="ruleSetName">Name of the rule set to load rules to.</param>
        /// <returns>Spec to continue fluent configuration.</returns>
        IRuleLoadSpec To(string ruleSetName);
    }

    internal class RuleLoadSpec : IRuleLoadSpec
    {
        private readonly IRuleActivator _activator;
        private readonly RuleTypeScanner _typeScanner = new RuleTypeScanner();
        private string _ruleSetName;

        public RuleLoadSpec(IRuleActivator activator)
        {
            _activator = activator;
        }

        public string RuleSetName
        {
            get { return _ruleSetName; }
        }

        public IRuleLoadSpec From(params Assembly[] assemblies)
        {
            _typeScanner.Assembly(assemblies);
            return this;
        }

        public IRuleLoadSpec From(params Type[] types)
        {
            _typeScanner.Type(types);
            return this;
        }

        public IRuleLoadSpec Where(Func<IRuleMetadata, bool> filter)
        {
            if (_typeScanner.IsFilterSet())
            {
                throw new InvalidOperationException("Rule load specification can only have a single 'Where' clause");
            }
            _typeScanner.Where(filter);
            return this;
        }

        public IRuleLoadSpec To(string ruleSetName)
        {
            if (_ruleSetName != null)
            {
                throw new InvalidOperationException("Rule load specification can only have a single 'To' clause");
            }
            _ruleSetName = ruleSetName;
            return this;
        }

        public IEnumerable<IRuleDefinition> Load()
        {
            var ruleDefinitions = _typeScanner
                .GetTypes()
                .Select(t => _activator.Activate(t))
                .Select(r => r.GetDefinition());
            return ruleDefinitions;
        }
    }
}
