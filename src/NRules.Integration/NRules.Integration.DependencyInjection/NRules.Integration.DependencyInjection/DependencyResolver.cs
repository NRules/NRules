using System;
using Microsoft.Extensions.DependencyInjection;
using NRules.Extensibility;

namespace NRules.Integration.DependencyInjection;

/// <summary>
/// Dependency resolver that uses .NET DI container.
/// </summary>
public class DependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a dependency resolver that uses the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">Service provider to use to resolve dependencies.</param>
    public DependencyResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    /// <inheritdoc cref="IDependencyResolver.Resolve"/>
    public object Resolve(IResolutionContext context, Type serviceType)
    {
        return _serviceProvider.GetRequiredService(serviceType);
    }
}