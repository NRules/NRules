using Autofac;
using NRules.Samples.ClaimsCenter.Presentation.Views;

namespace NRules.Samples.ClaimsCenter.Presentation.Modules
{
    public class PresentationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindow>()
                .AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ClaimListView>()
                .AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ClaimView>()
                .AsImplementedInterfaces().InstancePerDependency();
        }
    }
}