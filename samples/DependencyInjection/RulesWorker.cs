using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Extensions.Hosting;
using Timer = System.Timers.Timer;

namespace NRules.Samples.DependencyInjection;

internal class RulesWorker : IHostedService
{
    private readonly ISession _session;
    private readonly Timer _timer = new();

    public RulesWorker(ISession session)
    {
        _session = session;
        _timer.Enabled = false;
        _timer.Interval = 1000;
        _timer.AutoReset = true;
        _timer.Elapsed += TimerOnElapsed;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        _session.Insert(new TestFact{ TestProperty = "Test" });
        _session.Fire();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Stop();
        return Task.CompletedTask;
    }
}