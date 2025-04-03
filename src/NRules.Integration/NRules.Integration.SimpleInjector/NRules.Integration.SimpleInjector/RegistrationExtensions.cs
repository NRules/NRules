using System;
using System.Runtime.CompilerServices;
using NRules.Extensibility;
using NRules.Fluent;
using NRules.RuleModel;
using SimpleInjector;
using Container = SimpleInjector.Container;

namespace NRules.Integration.SimpleInjector;

public static class RegistrationExtensions
{
    /// <summary>
    /// RegisterRuleRepository the IRuleRepository.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="scanAction">The RuleTypeScanner action.</param>
    /// <param name="lifestyle">The SimpleInjector lifestyle. Default Singleton.</param>
    /// <returns></returns>
    public static Container RegisterRuleRepository(
        this Container container,
        Action<IRuleTypeScanner> scanAction,
        Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Singleton;
        
        // Scan for Rule(s)
        var scanner = new RuleTypeScanner();
        scanAction(scanner);
        var ruleTypes = scanner.GetRuleTypes();

        // Register all rules found
        foreach (var ruleType in ruleTypes)
        {
            if (container.GetRegistration(ruleType) == null)
            {
                container.Register(ruleType, ruleType, Lifestyle.Transient);
            }
        }

        // Regsiter IRuleActivator
        container.RegisterRuleActivator();
        
        // Register IDependencyResolver
        container.RegisterDependencyResolver();

        // Register RuleRepository
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
    public static Container RegisterRuleActivator(
        this Container container,
        Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Transient;
        if (container.GetRegistration(typeof(IRuleActivator)) == null)
        {
            container.Register<IRuleActivator, SimpleInjectorRuleActivator>(lifestyle);
        }
        return container; 
    }

    
     /// <summary>
     /// Register the IDependencyResolver with SimpleInjector as backing Container.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <param name="lifestyle">The lifestyle scope. Default Transient.</param>
     /// <returns></returns>
    public static Container RegisterDependencyResolver(
        this Container container,
        Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Transient;
        if (container.GetRegistration(typeof(IDependencyResolver)) == null)
        {
            container.Register<IDependencyResolver, SimpleInjectorDependencyResolver>(lifestyle);
        }
        return container; 
    }

     /// <summary>
     /// Register the SessionFactory.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <param name="lifestyle">The lifestyle scope. Default Singleton.</param>
     /// <returns></returns>
    public static Container RegisterSessionFactory(
        this Container container,
        Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Singleton;
        if (container.GetRegistration(typeof(ISessionFactory)) == null)
        {
            container.Register<ISessionFactory>(() =>
            {
                var ruleRepository = container.GetInstance<IRuleRepository>();
                var factory = ruleRepository.Compile();
                factory.DependencyResolver = container.GetInstance<IDependencyResolver>();
                return factory;
            }, lifestyle);
        }
        return container;
    }

    /// <summary>
    /// Register ISession through using instance of ISessionFactory.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="lifestyle">The Lifestyle for this Session object. Default Scoped.</param>
    /// <returns></returns>
    public static Container RegisterSession(
        this Container container,
        Lifestyle? lifestyle = null)
    {
        lifestyle ??= Lifestyle.Scoped;
        container.Register<ISession>(() => container.GetInstance<ISessionFactory>().CreateSession(), lifestyle);
        return container;
    }


    public static Container RegisterCompiledRuleSets<T>(
        this Container container)
        where T : class, ICompiledRuleSets<T>
    {
        container
            .RegisterRuleActivator()
            .RegisterDependencyResolver();
        
        
        var compiledRuleSets = Activator.CreateInstance<T>();
        compiledRuleSets.RuleActivator = new SimpleInjectorRuleActivator(container);
        var ruleTypes = compiledRuleSets.GetOrDefine().GetRuleTypes();
        foreach (var ruleType in ruleTypes)
        {
            container.Register(ruleType, ruleType, Lifestyle.Transient);
        }
        
        container.Register<ICompiledRuleSets<T>>(() =>
        {
            return compiledRuleSets;
        }, Lifestyle.Scoped);
        return container;
    }

    public static ICompiledRuleSets<T> GetCompiledRuleSets<T>(
        this Container container)
    {
        return container.GetInstance<ICompiledRuleSets<T>>();
    }
    
}