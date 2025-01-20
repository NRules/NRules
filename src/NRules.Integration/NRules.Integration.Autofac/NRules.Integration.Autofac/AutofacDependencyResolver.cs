using System;
using Autofac;
using NRules.Extensibility;

namespace NRules.Integration.Autofac;

/// <summary>
/// Dependency resolver that uses Autofac DI container.
/// </summary>
public class AutofacDependencyResolver : IDependencyResolver
{
    private readonly ILifetimeScope _container;

    /// <summary>
    /// Creates a dependency resolver that uses the specified container.
    /// </summary>
    /// <param name="container">Container to use to resolve dependencies.</param>
    public AutofacDependencyResolver(ILifetimeScope container)
    {
        _container = container;
    }

    public object Resolve(IResolutionContext context, Type serviceType)
    {
        return _container.Resolve(serviceType);
    }
}