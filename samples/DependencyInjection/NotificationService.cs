using System;

namespace NRules.Samples.DependencyInjection;

public interface INotificationService
{
    void NotifyRuleFired(string ruleName);
}

public class NotificationService : INotificationService
{
    public void NotifyRuleFired(string ruleName)
    {
        Console.WriteLine($"{ruleName} fired");
    }
}