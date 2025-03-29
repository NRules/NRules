using System;
using System.Collections.Generic;
using NRules.Fluent;
using NRules.RuleModel;
using SimpleInjector;

namespace NRules.Integration.SimpleInjector;

public class SimpleInjectorRuleRepositoryFactory : ISimpleInjectorRuleRepositoryFactory
{
    readonly Container container;

    readonly Dictionary<string, InstanceProducer<IRuleRepository>> producers =
        new Dictionary<string, InstanceProducer<IRuleRepository>>(
            StringComparer.OrdinalIgnoreCase);

    public SimpleInjectorRuleRepositoryFactory(Container container)
    {
        this.container = container;
        
        // Register the SimpleInjectorRuleActivator as IRuleActivator service
        if (container.GetRegistration<IRuleActivator>() == null)
        {
            container.Register<IRuleActivator, SimpleInjectorRuleActivator>(Lifestyle.Transient);
        }
    }

    public IRuleRepository CreateNew(string name) =>
        this.producers[name].GetInstance();

    public void RegisterNamedRuleRepository(
        string name,
        Action<IRuleTypeScanner> scanAction,
        Lifestyle? lifestyle = null,
        bool compileRules = true)
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

        var producer = lifestyle.CreateProducer<IRuleRepository>(() =>
        {
            var repo = new RuleRepository(new SimpleInjectorRuleActivator(container)); 
            repo.Load(s => s.From(ruleTypes));

            if (compileRules)
            {
                repo.Compile();
            }
            
            return repo;
        }, container);

        producers.Add(name, producer);
    }
}