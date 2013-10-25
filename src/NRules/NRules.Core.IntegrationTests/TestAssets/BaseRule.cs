using NRules.Fluent.Dsl;

namespace NRules.Core.IntegrationTests.TestAssets
{
    public abstract class BaseRule : IRule
    {
        public INotifier Notifier { get; set; }
        public abstract void Define(IDefinition definition);
    }
}