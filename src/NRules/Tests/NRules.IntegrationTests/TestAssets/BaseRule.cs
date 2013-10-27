using NRules.Dsl;

namespace NRules.IntegrationTests.TestAssets
{
    public abstract class BaseRule : IRule
    {
        public INotifier Notifier { get; set; }
        public abstract void Define(IDefinition definition);
    }
}