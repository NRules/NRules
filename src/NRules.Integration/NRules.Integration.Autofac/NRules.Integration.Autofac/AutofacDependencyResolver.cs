using System;
using Autofac;
using NRules.Extensibility;

namespace NRules.Integration.Autofac
{
    /// <summary>
    /// Dependency resolver that uses Autofac DI container.
    /// </summary>
    public class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _container;

        public AutofacDependencyResolver(ILifetimeScope container)
        {
            _container = container;
        }

        public object Resolve(IResolutionContext context, Type serviceType)
        {
            return _container.Resolve(serviceType);
        }
    }
}