using System;
using Autofac;

namespace NRules.Samples.ClaimsExpert.Service.Infrastructure
{
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _scope;

        public AutofacDependencyResolver(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public object Resolve(IResolutionContext context, Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }
    }
}
