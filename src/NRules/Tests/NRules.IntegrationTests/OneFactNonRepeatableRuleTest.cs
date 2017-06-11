using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneFactNonRepeatableRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFactEligibleForOneIncrement_FiresOnce()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1", TestCount = 2};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Fact]
        public void Fire_OneMatchingFactEligibleForTwoIncrements_FiresOnce()
        {
            //Arrange
            var fact = new FactType {TestProperty = "Valid Value 1", TestCount = 1};
            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
            public int TestCount { get; set; }

            public void IncrementCount()
            {
                TestCount++;
            }
        }

        [Repeatability(RuleRepeatability.NonRepeatable)]
        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType fact = null;

                When()
                    .Match<FactType>(() => fact, f => f.TestProperty.StartsWith("Valid"), f => f.TestCount <= 2);
                Then()
                    .Do(ctx => fact.IncrementCount())
                    .Do(ctx => ctx.Update(fact));
            }
        }
    }
}