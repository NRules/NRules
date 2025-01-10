using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NRules.Integration.DependencyInjection;

namespace NRules.Samples.DependencyInjection;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)  
            .ConfigureLogging((context, builder) => builder.AddConsole())
            .ConfigureServices((context, services) =>
            {
                services.AddHostedService<RulesWorker>();
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddNRules(scan => scan.Type(typeof(TestRule)));
            })
            .Build();

        await host.RunAsync();
    }
}