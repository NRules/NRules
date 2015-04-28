using System.Configuration;
using Autofac;
using NRules.Samples.ClaimsExpert.Contract;
using NRules.Samples.ClaimsExpert.Rules;
using NRules.Samples.ClaimsExpert.Service.Services;

namespace NRules.Samples.ClaimsExpert.Service.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var adjudicationServiceAddress = ConfigurationManager.AppSettings["adjudicationServiceAddress"];
            var claimServiceAddress = ConfigurationManager.AppSettings["claimServiceAddress"];
            builder.RegisterType<ServiceController>()
                .WithParameter(
                    (pi, c) => pi.Name == "adjudicationServiceAddress",
                    (pi, c) => adjudicationServiceAddress)
                .WithParameter(
                    (pi, c) => pi.Name == "claimServiceAddress",
                    (pi, c) => claimServiceAddress)
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<AdjudicationService>()
                .As<IAdjudicationService>().InstancePerDependency();
            builder.RegisterType<ClaimService>()
                .As<IClaimService>().InstancePerDependency();
            builder.RegisterType<NotificationService>()
                .As<INotificationService>().InstancePerDependency();
        }
    }
}