using System;
using Autofac;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Samples.ClaimsExpert.Service.Infrastructure
{
    public class AutofacRuleActivator : IRuleActivator
    {
        private readonly ILifetimeScope _scope;

        public AutofacRuleActivator(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public Rule Activate(Type type)
        {
            return (Rule)_scope.Resolve(type);
        }
    }
}