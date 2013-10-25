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

        public void Define(IDefinition definition)
        {
            TestFact1 fact1 = null;
            TestFact2 fact2 = null;

            definition.When()
                .If<TestFact1>(() => fact1, f => f.Name == "Hello")
                .If<TestFact2>(() => fact2, f => f.Fact1 == fact1);

            definition.Then()
                .Do(() => SaveResult(_a))
                .Do(() => SaveResult(_b));
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