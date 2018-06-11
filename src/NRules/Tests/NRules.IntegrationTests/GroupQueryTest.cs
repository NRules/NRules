using System;
using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class GroupQueryTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_MatchingSetOfFacts_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value"};
            var fact2 = new FactType2 {TestProperty = "Valid Value"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IEnumerable<Tuple<FactType1, IEnumerable<FactType2>>> match = null;

                When()
                    .Query(() => match, q => q
                        .Query(qq =>
                        {
                            FactType1 factType1 = null;

                            return qq
                                .Match<FactType1>(() => factType1, f => f.TestProperty.StartsWith("Valid"))
                                .Match<FactType2>(x => x.TestProperty == factType1.TestProperty)
                                .Collect()
                                .Select(x => Tuple.Create(factType1, x));
                        })
                        .Collect());
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}