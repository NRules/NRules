using NRules.Fluent.Dsl;

namespace NRules.Tests.TestAssets
{
    public class TestRule1 : Rule
    {
        private readonly string _a;
        private readonly string _b;
        public string Results { get; private set; }

        public TestRule1()
        {
            _a = "a";
            _b = "b";
            Results = string.Empty;
        }

        public TestRule1(string a, string b)
        {
            _a = a;
            _b = b;
            Results = string.Empty;
        }

        public override void Define()
        {
            TestFact1 fact1 = null;
            TestFact2 fact2 = null;

            When()
                .Match<TestFact1>(() => fact1, f => f.Name == "Hello")
                .Match<TestFact2>(() => fact2, f => f.Fact1 == fact1);

            Then()
                .Do(ctx => SaveResult(_a))
                .Do(ctx => SaveResult(_b));
        }

        private void SaveResult(string result)
        {
            Results += result;
        }
    }

    public class TestFact1
    {
        public string Name { get; set; }
    }

    public class TestFact2
    {
        public TestFact1 Fact1 { get; set; }
        public int Amount { get; set; }
    }
}