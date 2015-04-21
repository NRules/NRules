using Autofac;
using NRules.Fluent;
using NRules.RuleModel;
using NRules.Samples.ClaimsExpert.Rules;
using NRules.Samples.ClaimsExpert.Service.Infrastructure;

namespace NRules.Samples.ClaimsExpert.Service.Modules
{
    public class RulesEngineModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var typeScanner = new RuleTypeScanner();
            var ruleTypes = typeScanner.AssemblyOf(typeof(DslExtensions)).GetTypes();

            builder.RegisterTypes(ruleTypes).AsSelf();

            builder.RegisterType<AutofacRuleActivator>()
                .As<IRuleActivator>();

            builder.RegisterType<RuleRepository>()
                .As<IRuleRepository>()
                .PropertiesAutowired()
                .SingleInstance()
                .OnActivating(e => e.Instance.Load(x => x.From(ruleTypes)));

            builder.Register(c => c.Resolve<IRuleRepository>().Compile())
                .As<ISessionFactory>()
                .SingleInstance()
                .OnActivating(e => new RulesEngineLogger(e.Instance))
                .AutoActivate();
        }
    }
}