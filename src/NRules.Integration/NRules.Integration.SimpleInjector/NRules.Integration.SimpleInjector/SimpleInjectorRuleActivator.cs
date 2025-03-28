using System;
using System.Collections.Generic;
using System.Text;
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
            if (_container.GetRegistration(type) != null)
            {
                var collectionType = typeof(IEnumerable<>).MakeGenericType(type);
                return (IEnumerable<Rule>)_container.GetInstance(collectionType);
            }

            return ActivateDefault(type);
        }

        private static IEnumerable<Rule> ActivateDefault(Type type)
        {
            yield return (Rule) Activator.CreateInstance(type);
        }
    }
}
