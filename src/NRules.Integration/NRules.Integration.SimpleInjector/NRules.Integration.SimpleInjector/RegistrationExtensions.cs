using System;
using NRules.Extensibility;
using NRules.Fluent;
using NRules.RuleModel;
using SimpleInjector;
using Container = SimpleInjector.Container;

namespace NRules.Integration.SimpleInjector;

/// <summary>
/// Extension methods on <see cref="Container"/> to register NRules components with SimpleInjector container.
/// </summary>
/// <see href="https://docs.simpleinjector.org/en/latest/index.html"/>
public static class RegistrationExtensions
{
    /// <summary>
    /// Register the instance for IRuleRepository.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="scanAction">The RuleTypeScanner action.</param>
    /// <param name="lifestyle">The SimpleInjector lifestyle. Default Singleton.</param>
    /// <returns></returns>
    public static Container 
        RegisterRuleRepository(this Container container, Action<IRuleTypeScanner> scanAction, Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Singleton;
        
        var scanner = new RuleTypeScanner();
        scanAction(scanner);
        var ruleTypes = scanner.GetRuleTypes();

        foreach (var ruleType in ruleTypes)
        {
            container.Register(ruleType, ruleType, Lifestyle.Transient);
        }

        container.RegisterRuleActivator();
        container.RegisterDependencyResolver();

        container.Register(typeof(IRuleRepository),
            () =>
            {
                var repo = new RuleRepository(container.GetInstance<IRuleActivator>()); 
                repo.Load(s => s.From(ruleTypes));

                return repo;
            } ,
            lifestyle);
        
        return container;
    }


    /// <summary>
    /// Register IRuleActivator using SimpleInjector as backing Container.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="lifestyle">The lifestyle parameter. Default Transient.</param>
    /// <returns></returns>
    public static Container 
        RegisterRuleActivator(this Container container, Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Transient;
        container.Register<IRuleActivator, SimpleInjectorRuleActivator>(lifestyle);
        return container; 
    }

    /// <summary>
    /// Register the IDependencyResolver with SimpleInjector as backing Container.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="lifestyle">The lifestyle scope. Default Transient.</param>
    /// <returns></returns>
    public static Container 
         RegisterDependencyResolver(this Container container, Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Transient;
        container.Register<IDependencyResolver, SimpleInjectorDependencyResolver>(lifestyle);
        return container; 
    }

    /// <summary>
    /// Register the SessionFactory.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="lifestyle">The lifestyle scope. Default Singleton.</param>
    /// <returns></returns>
    public static Container 
         RegisterSessionFactory(this Container container, Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Singleton;
        container.Register<ISessionFactory>(() =>
        {
            var ruleRepository = container.GetInstance<IRuleRepository>();
            var factory = ruleRepository.Compile();
            factory.DependencyResolver = container.GetInstance<IDependencyResolver>();
            return factory;
        }, lifestyle);
        return container;
    }

    /// <summary>
    /// Register ISession through using instance of ISessionFactory.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="lifestyle">The Lifestyle for this Session object. Default Scoped.</param>
    /// <returns></returns>
    public static Container 
        RegisterSession(this Container container, Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Scoped;
        container.Register<ISession>(() => container.GetInstance<ISessionFactory>().CreateSession(), lifestyle);
        return container;
    }
}