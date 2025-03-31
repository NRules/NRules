using System;
using NRules.Extensibility;
using NRules.Fluent;
using NRules.RuleModel;
using SimpleInjector;
using Container = SimpleInjector.Container;

namespace NRules.Integration.SimpleInjector;

public static class RegistrationExtensions
{
    /// <summary>
    /// The Default interface tag. Used by Register functions without Generic Type Parameter.
    /// </summary>
    public interface IDefaultRuleRepository: IRuleRepository<IDefaultRuleRepository>
    {
    }
    
    /// <summary>
    /// RegisterRuleRepository the generic version for custom IRuleRepository.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <param name="scanAction">The RuleTypeScanner action.</param>
    /// <param name="lifestyle">The SimpleInjector lifestyle. Default Singleton.</param>
    /// <typeparam name="TService">The Interface Service to use as "Tag"</typeparam>
    /// <returns></returns>
    public static Container RegisterRuleRepository<TService>(
        this Container container,
        Action<IRuleTypeScanner> scanAction,
        Lifestyle? lifestyle = null)
        where TService : class, IRuleRepository<TService>
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

        // Register TService with custom RuleRepository scan as concrete type
        container.Register(typeof(IRuleRepository<TService>),
            () =>
            {
                var repo = new RuleRepositoryWrapper<TService>(new SimpleInjectorRuleActivator(container)); 
                repo.Load(s => s.From(ruleTypes));

                return repo;
            } ,
            lifestyle);
        
        return container;
    }
    /// <summary>
    /// RegisterRuleRepository the default version using IDefaultRuleRepository interface type.
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
        return container.RegisterRuleRepository<IDefaultRuleRepository>(scanAction, lifestyle);
    }


    /// <summary>
    /// Resolve RuleRepository with custom IRuleRepository generic type parameter from SimpleInjector.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <typeparam name="TService">The interface service tag type parameter.</typeparam>
    /// <returns></returns>
    public static IRuleRepository<TService> ResolveRuleRepository<TService>(
        this Container container)
        where TService : class, IRuleRepository<TService>
    {
        return container.GetInstance<IRuleRepository<TService>>();
    }
    /// <summary>
    /// Resolve RuleRepository with default IDefaultRuleRepository type parameter from SimpleInjector.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <returns></returns>
    public static IRuleRepository ResolveRuleRepository(
        this Container container)
    {
        return container.GetInstance<IRuleRepository<IDefaultRuleRepository>>();
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
            container.Register<IRuleActivator>(() => new SimpleInjectorRuleActivator(container), lifestyle);
        }
        return container; 
    }

    /// <summary>
    /// Resolve the rule activator registered with SimnpleInjector.
    /// </summary>
    /// <param name="container">The SimpleInjector container.</param>
    /// <returns>The IRuleActivator object.</returns>
    public static IRuleActivator ResolveRuleActivator(this Container container)
    {
        return container.GetInstance<IRuleActivator>();
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
            container.Register<IDependencyResolver>(() => new SimpleInjectorDependencyResolver(container), lifestyle);
        }
        return container; 
    }

     /// <summary>
     /// Resolve the DependencyResolver registered with SimpleInjector container.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <returns>The DependencyResolver object.</returns>
    public static IDependencyResolver ResolveDependencyResolver(this Container container)
    {
        return container.GetInstance<IDependencyResolver>();
    }


     /// <summary>
     /// Register the generic version of SessionFactory.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <param name="lifestyle">The lifestyle scope. Default Singleton.</param>
     /// <typeparam name="TService">The interface service tag.</typeparam>
     /// <returns></returns>
    public static Container RegisterSessionFactory<TService>(
        this Container container,
        Lifestyle? lifestyle = null)
        where TService : class, IRuleRepository<TService>
    {
        lifestyle ??= Lifestyle.Singleton;
        if (container.GetRegistration(typeof(ISessionFactory<TService>)) == null)
        {
            container.Register<ISessionFactory<TService>>(() =>
            {
                return new SessionFactoryWrapper<TService>(container);
            }, lifestyle);
        }
        return container;
    }
     /// <summary>
     /// Register the default version of SessionFactory using IDefaultRuleRepository as service tag.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <param name="lifestyle">The lifestyle scope. Default Singleton.</param>
     /// <returns></returns>
    public static Container RegisterSessionFactory(
        this Container container,
        Lifestyle? lifestyle = null)
    {
        return container.RegisterSessionFactory<IDefaultRuleRepository>(lifestyle);
    }

   
     /// <summary>
     /// Resolve the generic version of the registered SessionFactory.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <typeparam name="TService">The interface service tag type parameter.</typeparam>
     /// <returns></returns>
    public static ISessionFactory<TService> ResolveSessionFactory<TService>(this Container container)
        where TService : class, IRuleRepository<TService>
    {
        return container.GetInstance<ISessionFactory<TService>>();
    }
     /// <summary>
     /// Resolve the default version of the registered SessionFactory.
     /// </summary>
     /// <param name="container">The SimpleInjector container.</param>
     /// <returns></returns>
    public static ISessionFactory ResolveSessionFactory(this Container container)
    {
        return container.GetInstance<ISessionFactory<IDefaultRuleRepository>>();
    }
}