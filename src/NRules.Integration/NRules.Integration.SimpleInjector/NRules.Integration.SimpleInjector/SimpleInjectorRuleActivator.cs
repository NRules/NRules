using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using SimpleInjector;

namespace NRules.Integration.SimpleInjector
{
    /// <summary>
    /// Rule activator that uses SimpleInjector DI container.
    /// </summary>
    public class SimpleInjectorRuleActivator : IRuleActivator
    {
        private readonly Container _container;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public SimpleInjectorRuleActivator(Container container)
        {
            _container = container;
        }

        public IEnumerable<Rule> Activate(Type type)
        {
            var rules = new List<Rule>();
            var instance = (_container.GetInstance(type) ?? Activator.CreateInstance(type)) as Rule;
            if (instance != null)
            {
                rules.Add(instance);
            }

            return rules.AsEnumerable();
        }

        private static Rule ActivateDefault(Type type)
        {
            return (Rule) Activator.CreateInstance(type);
        }
    }
}
