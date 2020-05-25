using BenchmarkDotNet.Attributes;
using NRules.Fluent.Dsl;

namespace NRules.Benchmark.Meta
{
    [BenchmarkCategory("Meta")]
    public class BenchmarkOneFactRule : BenchmarkBase
    {
        private TestFact[] _facts;

        [GlobalSetup]
        public void Setup()
        {
            SetupRule<TestRule>();

            _facts = new TestFact[FactCount];
            for (int i = 0; i < FactCount; i++)
            {
                _facts[i] = new TestFact{IntProperty = i};
            }
        }

        [Params(10, 100, 1000)]
        public int FactCount { get; set; }

        [Benchmark]
        public int Insert()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts);
            return session.Fire();
        }

        [Benchmark]
        public int InsertUpdate()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts);
            session.UpdateAll(_facts);
            return session.Fire();
        }

        [Benchmark]
        public int InsertRetract()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts);
            session.RetractAll(_facts);
            return session.Fire();
        }

        private class TestFact
        {
            public int IntProperty { get; set; }
        }

        private class TestRule : Rule
        {
            public override void Define()
            {
                TestFact fact = null;

                When()
                    .Match(() => fact, 
                        x => x.IntProperty % 2 == 0);

                Then()
                    .Do(_ => Nothing());
            }

            private void Nothing()
            {
            }
        }
    }
}