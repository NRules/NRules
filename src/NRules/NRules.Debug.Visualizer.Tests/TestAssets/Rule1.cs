using NRules.Fluent.Dsl;

namespace NRules.Debug.Visualizer.Tests.TestAssets
{
    public class Rule1 : Rule
    {
        public override void Define()
        {
            Fact1 fact = null;

            When()
                .Match(() => fact, x => x.Value == "TestValue");

            Then()
                .Do(ctx => ctx.Halt());
        }
    }
}