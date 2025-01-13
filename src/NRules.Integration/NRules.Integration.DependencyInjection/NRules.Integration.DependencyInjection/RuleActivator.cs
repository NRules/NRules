using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Integration.DependencyInjection;

/// <summary>
/// Rule activator that uses .NET DI container.
/// </summary>
public class RuleActivator : IRuleActivator
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates a rule activator that uses the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">Service provider to use to create rule instances.</param>
    public RuleActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc cref="IRuleActivator.Activate"/>
    public IEnumerable<Rule> Activate(Type type)
    {
        var requestedService = _serviceProvider.GetServices(type);

        bool activateDefault = true;
        foreach (var service in requestedService)
        {
            if (service == null)
                continue;
            
            activateDefault = false;
            yield return (Rule) service;
        }

        if (activateDefault)
            yield return ActivateDefault(type);
    }

    private static Rule ActivateDefault(Type type)
    { 
        return (Rule) Activator.CreateInstance(type);
    }
}