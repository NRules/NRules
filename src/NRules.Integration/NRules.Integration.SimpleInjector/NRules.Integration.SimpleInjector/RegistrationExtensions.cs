using System;
using NRules.Fluent;
using NRules.RuleModel;
using SimpleInjector;
using Container = SimpleInjector.Container;

namespace NRules.Integration.SimpleInjector;

public static class RegistrationExtensions
{
    private static SimpleInjectorRuleRepositoryFactory? Factory { get; set; }
    
    public static void RegisterNamedRuleRepository(
        this Container container,
        Action<IRuleTypeScanner> scanAction,
        string name = "default",
        Lifestyle? lifestyle = null,
        bool compileRules = true)
    {
        lifestyle ??= Lifestyle.Singleton;

        if (Factory == null)
        {
            Factory = new SimpleInjectorRuleRepositoryFactory(container);
        }
        
        Factory.RegisterNamedRuleRepository(name, scanAction, lifestyle, compileRules);
    }

    public static IRuleRepository GetNamedRuleRepository(
        this Container container,
        string name = "default")
    {
        if (Factory == null)
        {
            throw new InvalidOperationException("Call RegisterNamedRuleRepository to register a rule repository first!");
        }
        
        return Factory.CreateNew(name);
    }
}