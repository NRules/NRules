using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;

namespace NRules.Debugger.Visualizer.Tests.TestAssets
{
    public class Fact1
    {
        public string TestProperty { get; set; }
    }

    public class Fact2
    {
        public string TestProperty { get; set; }
        public string JoinProperty { get; set; }
    }

    public class Fact3
    {
        public string TestProperty { get; set; }
    }

    public class Fact4
    {
        public string TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public override void Define()
        {
            Fact1 fact1 = null;
            string joinValue = null;
            Fact2 fact2 = null;
            IEnumerable<Fact4> group = null;

            When()
                .Match<Fact1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Calculate(() => joinValue, () => fact1.TestProperty)
                .Match<Fact2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == joinValue)
                .Not<Fact3>(f => f.TestProperty.StartsWith("Invalid"))
                .Exists<Fact3>(f => f.TestProperty.StartsWith("Valid"))
                .Query(() => group, q => q
                    .Match<Fact4>()
                    .Where(f => f.TestProperty.StartsWith("Valid"))
                    .GroupBy(f => f.TestProperty)
                    .SelectMany(x => x)
                    .Collect()
                    .Where(c => c.Any()));

            Then()
                .Do(ctx => NoOp());
        }

        private void NoOp()
        {
        }
    }
}