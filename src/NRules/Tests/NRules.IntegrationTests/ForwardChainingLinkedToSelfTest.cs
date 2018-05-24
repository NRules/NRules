using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ForwardChainingLinkedToSelfTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_TwoMatchingSetsOfFactsYieldsThenRetracted_FiresAndYieldedFactsAreRemoved()
        {
            //Arrange
            var order1 = new FactType1 { GroupKey = "Group 1" };
            var order2 = new FactType1 { GroupKey = "Group 2" };
            Session.InsertAll(new[] {order1, order2});
            
            //Act
            Session.Fire();
            Session.Retract(order1);
            Session.Retract(order2);

            //Assert
            AssertFiredTimes(3);
            Assert.Equal(0, Session.Query<FactType4>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<ForwardChainingRule>();
        }

        public class FactType1
        {
            public string GroupKey { get; set; }
        }

        public class FactType2
        {
            public string Key { get; set; }
        }

        public class FactType3
        {
            public int Value { get; set; }
        }

        public class FactType4
        {
            public int Value { get; set; }
        }

        public class ForwardChainingRule : Rule
        {
            public override void Define()
            {
                IGrouping<string, FactType1> facts = null;
                IEnumerable<FactType2> otherFacts = null;
                FactType4 projection = null;

                When()
                    .Query(() => facts, q => q
                        .Match<FactType1>()
                        .GroupBy(x => x.GroupKey))
                    .Query(() => otherFacts, q => q
                        .Match<FactType2>(x => x.Key != facts.Key)
                        .Collect())
                    .Query(() => projection, q => q
                        .Match<FactType3>()
                        .Collect()
                        .Select(x => ProjectValue(x)));

                Filter()
                    .OnChange(() => otherFacts.Count());

                Then()
                    .Yield(ctx => Create(facts));
            }

            private static FactType4 ProjectValue(IEnumerable<FactType3> x)
            {
                return new FactType4 {Value = x.Select(p => p.Value).FirstOrDefault()};
            }

            private static FactType2 Create(IGrouping<string, FactType1> orders)
            {
                return new FactType2 {Key = orders.Key};
            }
        }
    }
}