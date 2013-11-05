using NRules.Fluent.Dsl;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRule : Rule
    {
        public INotifier Notifier { get; set; }
    }
}