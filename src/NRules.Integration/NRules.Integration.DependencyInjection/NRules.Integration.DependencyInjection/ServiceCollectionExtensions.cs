using System;
using Microsoft.Extensions.DependencyInjection;
using NRules.Extensibility;
using NRules.Fluent;
using NRules.RuleModel;

namespace NRules.Integration.DependencyInjection;

/// <summary>
/// Extension methods on <see cref="IServiceCollection"/> to register the necessary rules engine services and types.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds necessary NRules components to the services collection.
    /// </summary>
    /// <param name="services">Services collection to add NRules components to.</param>
    /// <param name="scanAction">Configuration action on the rule type scanner.</param>
    public static void AddNRules(this IServiceCollection services, Action<IRuleTypeScanner> scanAction)
    {
        var scanner = new RuleTypeScanner();
        scanAction(scanner);
        var ruleTypes = scanner.GetRuleTypes();

        foreach (var ruleType in ruleTypes)
        {
            services.AddTransient(ruleType);
        }

        services.AddTransient<IRuleActivator, RuleActivator>();
        
        services.AddSingleton<IRuleRepository>(serviceProvider =>
        {
            var ruleRepository = new RuleRepository();
            ruleRepository.Activator = serviceProvider.GetRequiredService<IRuleActivator>();
            ruleRepository.Load(x => x.From(ruleTypes));
            return ruleRepository;
        });

        services.AddTransient<IDependencyResolver, DependencyResolver>();
        
        services.AddSingleton<ISessionFactory>(serviceProvider =>
        {
            var ruleRepository = serviceProvider.GetRequiredService<IRuleRepository>();
            return ruleRepository.Compile();
        });
        
        services.AddTransient<ISession>(serviceProvider =>
        {
            var sessionFactory = serviceProvider.GetRequiredService<ISessionFactory>();
            var session = sessionFactory.CreateSession();
            session.DependencyResolver = serviceProvider.GetRequiredService<IDependencyResolver>();
            return session;
        });
    }
}