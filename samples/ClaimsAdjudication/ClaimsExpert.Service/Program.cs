using Autofac;
using Common.Logging;
using NRules.Samples.ClaimsExpert.Domain;
using NRules.Samples.ClaimsExpert.Domain.Modules;
using NRules.Samples.ClaimsExpert.Service.Modules;
using Topshelf;

namespace NRules.Samples.ClaimsExpert.Service
{
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger<ServiceController>();

        private static IContainer Container { get; set; }

        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<IServiceController>(s =>
                {
                    s.ConstructUsing(name => BuildServiceController());
                    s.WhenStarted(sc => sc.Start());
                    s.WhenStopped(sc => Container.Dispose());
                    s.AfterStartingService(() => Log.Info("Claims expert service started"));
                    s.AfterStoppingService(() => Log.Info("Claims expert service stopped"));
                });
                x.RunAsLocalSystem();

                x.SetDescription("Claims expert service");
                x.SetDisplayName("Claims Expert");
                x.SetServiceName("ClaimsExpert");
            });
        }

        private static IServiceController BuildServiceController()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyModules(typeof(ServiceModule).Assembly);
            containerBuilder.RegisterAssemblyModules(typeof(DomainModule).Assembly);
            Container = containerBuilder.Build();
            
            return Container.Resolve<IServiceController>();
        }
    }
}