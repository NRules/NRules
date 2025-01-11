using NRules.Fluent.Dsl;

namespace NRules.Samples.DependencyInjection;

public class TestRule : Rule
{
    private readonly INotificationService _notificationService;

    public TestRule(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    public override void Define()
    {
        TestFact fact = default!;
        
        When()
            .Match(() => fact, x => x.TestProperty == "Test");

        Then()
            .Do(ctx => _notificationService.NotifyRuleFired(ctx.Rule.Name));
    }
}