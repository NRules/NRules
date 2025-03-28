using System;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using SimpleInjector;

namespace NRules.Integration.SimpleInjector;

public static class RegistrationExtensions
{
    public static void RegisterRuleRepository(
        this Container builder,
        Action<IRuleTypeScanner> scanAction)
    {
        var scanner = new RuleTypeScanner();
        scanAction(scanner);
        var ruleTypes = scanner.GetRuleTypes();

        builder.Collection.CreateRegistration<Rule>(ruleTypes);
        
        builder.Register<IRuleActivator, SimpleInjectorRuleActivator>(Lifestyle.Transient);

        //builder.RegisterInitializer<IRuleRepository>((rr) => new RuleRepository(new SimpleInjectorRuleActivator(builder)));
        builder.Register<IRuleRepository>(() => new RuleRepository(), Lifestyle.Singleton);
    }
}