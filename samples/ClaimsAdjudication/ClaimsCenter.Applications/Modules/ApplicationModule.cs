using System.Waf.Presentation.Services;
using Autofac;
using NRules.Samples.ClaimsCenter.Applications.Controllers;
using NRules.Samples.ClaimsCenter.Applications.ViewModels;

namespace NRules.Samples.ClaimsCenter.Applications.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MessageService>()
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<ApplicationController>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AdjudicationController>()
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<ClaimController>()
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MainViewModel>()
                .AsSelf().SingleInstance();
            builder.RegisterType<ClaimListViewModel>()
                .AsSelf().SingleInstance();
            builder.RegisterType<ClaimViewModel>()
                .AsSelf().SingleInstance();
        }
    }
}