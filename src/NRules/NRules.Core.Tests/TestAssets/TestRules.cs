using NRules.Dsl;

namespace NRules.Core.Tests.TestAssets
{
    public class TestRule1 : IRule
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

        public void Define(IRuleDefinition definition)
        {
            definition.When()
                .If<TestFact1>(fact1 => fact1.Name == "Hello")
                .If<TestFact1, TestFact2>((fact1, fact2) => fact2.Fact1 == fact1);

            definition.Then()
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