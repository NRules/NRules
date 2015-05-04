using Autofac;

namespace NRules.Samples.ClaimsExpert.Domain.Modules
{
    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ClaimRepository>()
                .AsImplementedInterfaces().InstancePerDependency();
        }
    }
}
