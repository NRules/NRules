using System;
using System.Collections.Generic;
using Autofac;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Integration.Autofac
{
    /// <summary>
    /// Rule activator that uses Autofac DI container.
    /// </summary>
    public class AutofacRuleActivator : IRuleActivator
    {
        private readonly ILifetimeScope _container;

        public AutofacRuleActivator(ILifetimeScope container)
        {
            _container = container;
        }

        public IEnumerable<Rule> Activate(Type type)
        {
            if (_container.IsRegistered(type)) 
                yield return (Rule) _container.Resolve(type);

            yield return (Rule)Activator.CreateInstance(type);
        }
    }
}