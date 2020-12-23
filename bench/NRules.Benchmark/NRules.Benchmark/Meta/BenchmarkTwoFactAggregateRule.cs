using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using NRules.Fluent.Dsl;

namespace NRules.Benchmark.Meta
{
    [BenchmarkCategory("Meta")]
    public class BenchmarkTwoFactAggregateRule : BenchmarkBase
    {
        private TestFact1[] _facts1;
        private TestFact2[] _facts2;

        [GlobalSetup]
        public void Setup()
        {
            SetupRule<TestRule>();

            _facts1 = new TestFact1[Fact1Count];
            for (int i = 0; i < Fact1Count; i++)
            {
                _facts1[i] = new TestFact1{IntProperty = i};
            }

            _facts2 = new TestFact2[Fact2Count];
            for (int i = 0; i < Fact2Count; i++)
            {
                _facts2[i] = new TestFact2{IntProperty = i};
            }
        }

        [Params(10, 100, 1000)]
        public int Fact1Count { get; set; }

        [Params(10, 100, 1000)]
        public int Fact2Count { get; set; }

        [Benchmark]
        public int Insert()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts1);
            session.InsertAll(_facts2);
            return session.Fire();
        }

        [Benchmark]
        public int InsertUpdate()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts1);
            session.InsertAll(_facts2);
            session.UpdateAll(_facts1);
            session.UpdateAll(_facts2);
            return session.Fire();
        }

        [Benchmark]
        public int InsertRetract()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts1);
            session.InsertAll(_facts2);
            session.RetractAll(_facts1);
            session.RetractAll(_facts2);
            return session.Fire();
        }

        private class TestFact1
        {
            public int IntProperty { get; set; }
        }

        private class TestFact2
        {
            public int IntProperty { get; set; }
        }

        private class TestRule : Rule
        {
            public override void Define()
            {
                TestFact1 fact1 = null;
                IEnumerable<TestFact2> group = null;

                When()
                    .Match(() => fact1,
                        x => x.IntProperty % 2 == 0)
                    .Query(() => group, q => q
                        .Match<TestFact2>(
                            x => x.IntProperty % 2 == 0)
                        .Where(x => x.IntProperty % 2 == fact1.IntProperty % 4)
                        .GroupBy(x => x.IntProperty % 10));

                Then()
                    .Do(_ => Nothing());
            }

            private void Nothing()
            {
            }
        }
    }
}