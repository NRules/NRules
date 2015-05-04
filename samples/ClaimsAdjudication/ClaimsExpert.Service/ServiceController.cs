using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using Autofac;
using Autofac.Integration.Wcf;
using Common.Logging;
using NRules.Samples.ClaimsExpert.Contract;
using NRules.Samples.ClaimsExpert.Service.Infrastructure;
using NRules.Samples.ClaimsExpert.Service.Services;

namespace NRules.Samples.ClaimsExpert.Service
{
    public interface IServiceController
    {
        void Start();
    }

    internal class ServiceController : IServiceController, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger<ServiceController>();

        private readonly ILifetimeScope _container;
        private readonly Uri _adjudicationServiceAddress;
        private readonly Uri _claimServiceAddress;
        private readonly List<ServiceHost> _serviceHosts = new List<ServiceHost>();

        public ServiceController(ILifetimeScope container, string adjudicationServiceAddress, string claimServiceAddress)
        {
            _container = container;
            _adjudicationServiceAddress = new Uri(adjudicationServiceAddress);
            _claimServiceAddress = new Uri(claimServiceAddress);
        }

        public void Start()
        {
            _serviceHosts.Add(CreateHost<AdjudicationService, IAdjudicationService>(_adjudicationServiceAddress));
            _serviceHosts.Add(CreateHost<ClaimService, IClaimService>(_claimServiceAddress));
        }

        public void Stop()
        {
            foreach (var serviceHost in _serviceHosts)
            {
                serviceHost.Close();
                Log.InfoFormat("The WCF host has been closed. Name={0}", serviceHost.Description.Name);
            }
            _serviceHosts.Clear();
        }

        public void Dispose()
        {
            Stop();
        }

        private ServiceHost CreateHost<TService, TContract>(Uri address)
        {
            Log.InfoFormat("Exposing service. Address={0}", address);
            var host = new ServiceHost(typeof(TService), address);

            host.AddServiceEndpoint(typeof(TContract), new BasicHttpBinding(), string.Empty);
            host.AddDependencyInjectionBehavior<TContract>(_container);
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true, HttpGetUrl = address });
            host.Description.Behaviors.Add(new LoggingServiceBehavior());

            host.Open();
            Log.InfoFormat("The WCF host has been opened. Name={0}", host.Description.Name);
            return host;
        }
    }
}