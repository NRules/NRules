using NRules.Fluent.Dsl;

namespace NRules.Debugger.Visualizer.Tests.TestAssets
{
    public class Rule1 : Rule
    {
        public override void Define()
        {
            When()
                .Match<Fact1>(x => x.Value == "TestValue");

            Then()
                .Do(ctx => ctx.Halt());
        }
    }
}