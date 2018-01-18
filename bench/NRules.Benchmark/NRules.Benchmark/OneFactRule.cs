using BenchmarkDotNet.Attributes;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Benchmark
{
    public class OneFactRule
    {
        private ISessionFactory _factory;
        private TestFact[] _facts;

        [GlobalSetup]
        public void Setup()
        {
            var repository = new RuleRepository();
            repository.Load(x => x.NestedTypes().PrivateTypes().From(xx => xx.Type(typeof(TestRule))));

            _factory = repository.Compile();

            _facts = new TestFact[Count];
            for (int i = 0; i < Count; i++)
            {
                _facts[i] = new TestFact{Test = $"Valid {i}"};
            }
        }

        [Params(10, 100, 1000, 10000)]
        public int Count { get; set; }

        [Benchmark]
        public int Insert()
        {
            var session = _factory.CreateSession();
            session.InsertAll(_facts);
            return session.Fire();
        }

        [Benchmark]
        public int InsertUpdate()
        {
            var session = _factory.CreateSession();
            session.InsertAll(_facts);
            session.UpdateAll(_facts);
            return session.Fire();
        }

        [Benchmark]
        public int InsertRetract()
        {
            var session = _factory.CreateSession();
            session.InsertAll(_facts);
            session.RetractAll(_facts);
            return session.Fire();
        }

        private class TestFact
        {
            public string Test { get; set; }
        }

        private class TestRule : Rule
        {
            public override void Define()
            {
                TestFact fact = null;

                When()
                    .Match(() => fact, x => x.Test.StartsWith("Valid"));

                Then()
                    .Do(_ => Nothing());
            }

            private void Nothing()
            {
            }
        }
    }
}