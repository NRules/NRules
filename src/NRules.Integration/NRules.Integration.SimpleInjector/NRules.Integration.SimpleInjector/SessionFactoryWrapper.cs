using System;
using NRules.Diagnostics;
using NRules.Extensibility;
using SimpleInjector;

namespace NRules.Integration.SimpleInjector;

public interface ISessionFactory<TService> : ISessionFactory 
    where TService : class;

public class SessionFactoryWrapper<TService>: ISessionFactory<TService>
    where TService : class, IRuleRepository<TService>
{
    private readonly ISessionFactory factoryImpl;
    
    public SessionFactoryWrapper(Container container)
    {
        var ruleRepository = container.ResolveRuleRepository<TService>();
        factoryImpl = ruleRepository.Compile();
    }

    public ReteGraph GetSchema()
    {
        return factoryImpl.GetSchema();
    }

    public ISession CreateSession()
    {
        return factoryImpl.CreateSession();
    }

    public ISession CreateSession(Action<ISession> initializationAction)
    {
        return factoryImpl.CreateSession(initializationAction);
    }

    public IEventProvider Events { get => factoryImpl.Events; }
    public IDependencyResolver DependencyResolver { get => factoryImpl.DependencyResolver; set => factoryImpl.DependencyResolver = value; }
    public IActionInterceptor? ActionInterceptor { get => factoryImpl.ActionInterceptor; set => factoryImpl.ActionInterceptor = value; }
}
