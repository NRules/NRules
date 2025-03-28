using System;
using NRules.Extensibility;
using SimpleInjector;

namespace NRules.Integration.SimpleInjector
{
    public class SimpleInjectorDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;

        /// <summary>
        /// Creates a dependency resolver that uses the specified SimpleInjector container.
        /// </summary>
        /// <param name="container">Container to use to resolve dependencies.</param>
        public SimpleInjectorDependencyResolver(Container container)
        {
            _container = container;
        }

        public object Resolve(IResolutionContext context, Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}
