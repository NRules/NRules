using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class PriorityTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_LowPriorityActivatesTwiceTriggersHighPriority_HighPriorityPreemptsLowPriority()
        {
            //Arrange
            var invokedRules = new List<string>();

            Session.Events.RuleFiredEvent += (sender, args) => invokedRules.Add(args.Rule.Name);

            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType1 {TestProperty = "Valid Value 2"};
            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
                //low priority activates twice
                //it runs once, activates high priority rule, which preempts low priority and fires once
                //low priority fires second time, which activates high priority which also fires second time
            Assert.Equal(4, invokedRules.Count);
            Assert.Equal("PriorityLowRule", invokedRules[0]);
            Assert.Equal("PriorityHighRule", invokedRules[1]);
            Assert.Equal("PriorityLowRule", invokedRules[2]);
            Assert.Equal("PriorityHighRule", invokedRules[3]);
        }

        protected override void SetUpRules()
        {
            SetUpRule<PriorityLowRule>();
            SetUpRule<PriorityHighRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        [Name("PriorityLowRule")]
        [Priority(10)]
        public class PriorityLowRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.Insert(new FactType2()
                    {
                        TestProperty = "Valid Value",
                        JoinProperty = fact1.TestProperty
                    }));
            }
        }

        [Name("PriorityHighRule")]
        [Priority(100)]
        public class PriorityHighRule : Rule
        {
            public override void Define()
            {
                FactType2 fact2 = null;

                When()
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}