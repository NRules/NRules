using Autofac;
using NRules.Integration.Autofac;
using NRules.Samples.ClaimsExpert.Rules;
using NRules.Samples.ClaimsExpert.Service.Infrastructure;

namespace NRules.Samples.ClaimsExpert.Service.Modules
{
    public class RulesEngineModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var types = builder.RegisterRules(x => x.AssemblyOf(typeof(DslExtensions)));
            builder.RegisterRepository(r => r.Load(x => x.From(types)));
            builder.RegisterSessionFactory()
                .OnActivating(e => new RulesEngineLogger(e.Instance))
                .AutoActivate();
        }
    }
}