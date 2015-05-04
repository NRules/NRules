using System;
using System.Configuration;
using System.ServiceModel;
using Autofac;
using Autofac.Integration.Wcf;
using NRules.Samples.ClaimsExpert.Contract;

namespace NRules.Samples.ClaimsCenter.Applications.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var adjudicationServiceAddress = ConfigurationManager.AppSettings["adjudicationServiceAddress"];
            builder
                .Register(c => new ChannelFactory<IAdjudicationService>(
                    new BasicHttpBinding(),
                    new EndpointAddress(adjudicationServiceAddress)))
                .SingleInstance();
            builder
                .Register(c => c.Resolve<ChannelFactory<IAdjudicationService>>().CreateChannel())
                .As<IAdjudicationService>()
                .UseWcfSafeRelease();

            var claimServiceAddress = ConfigurationManager.AppSettings["claimServiceAddress"];
            builder
                .Register(c => new ChannelFactory<IClaimService>(
                    new BasicHttpBinding(),
                    new EndpointAddress(claimServiceAddress)))
                .SingleInstance();
            builder
                .Register(c => c.Resolve<ChannelFactory<IClaimService>>().CreateChannel())
                .As<IClaimService>()
                .UseWcfSafeRelease();
        }
    }
}