using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Benchmark.Meta
{
    [BenchmarkCategory("Meta")]
    public class BenchmarkMultipleRules : BenchmarkBase
    {
        private TestFact1[] _facts1;

        [GlobalSetup]
        public void Setup()
        {
            var rules = new List<IRuleDefinition>();
            for (int i = 1; i <= RuleCount; i++)
            {
                var rule = BuildRule(i);
                rules.Add(rule);
            }

            var compiler = new RuleCompiler();
            Factory = compiler.Compile(rules);

            _facts1 = new TestFact1[FactCount];
            for (int i = 0; i < FactCount; i++)
            {
                _facts1[i] = new TestFact1{IntProperty = i};
            }
        }

        [Params(10, 100, 1000)]
        public int RuleCount { get; set; }

        [Params(100)]
        public int FactCount { get; set; }

        [Benchmark]
        public int Insert()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts1);
            return session.Fire();
        }

        [Benchmark]
        public int InsertUpdate()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts1);
            session.UpdateAll(_facts1);
            return session.Fire();
        }

        [Benchmark]
        public int InsertRetract()
        {
            var session = Factory.CreateSession();
            session.InsertAll(_facts1);
            session.RetractAll(_facts1);
            return session.Fire();
        }

        private class TestFact1
        {
            public int IntProperty { get; set; }
        }

        private IRuleDefinition BuildRule(int index)
        {
            var builder = new RuleBuilder();
            builder.Name($"TestRule{index}");

            var group = builder.LeftHandSide();
            var pattern = group.Pattern(typeof(TestFact1), "fact1");
            Expression<Func<TestFact1, bool>> condition = fact1 => fact1.IntProperty % index == 0;
            pattern.Condition(condition);

            var actions = builder.RightHandSide();
            Expression<Action<IContext>> action = ctx => Nothing();
            actions.Action(action);

            return builder.Build();
        }

        private static void Nothing()
        {
        }
    }
}