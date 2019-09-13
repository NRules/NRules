using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class LinkedFactsErrorTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_YieldThrowsThenUpdatedToValid_YieldsOnUpdate()
        {
            //Arrange
            var fact1 = new FactType1 { ChainProperty = "Value", ShouldThrow = true};
            Session.Insert(fact1);

            Assert.Throws<RuleRhsExpressionEvaluationException>(() => Session.Fire());

            fact1.ShouldThrow = false;

            //Act
            Session.Update(fact1);
            Session.Fire();

            //Assert
            var linkedFacts = Session.Query<FactType2>();
            Assert.Equal(1, linkedFacts.Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<ForwardChainingFirstRule>();
        }

        public class FactType1
        {
            public string ChainProperty { get; set; }
            public bool ShouldThrow { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
        }

        public class ForwardChainingFirstRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;

                When()
                    .Match<FactType1>(() => fact1);
                Then()
                    .Yield(ctx => Create(fact1));
            }

            private static FactType2 Create(FactType1 fact1)
            {
                if (fact1.ShouldThrow) throw new InvalidOperationException();
                var fact2 = new FactType2 {TestProperty = fact1.ChainProperty};
                return fact2;
            }
        }
    }
}