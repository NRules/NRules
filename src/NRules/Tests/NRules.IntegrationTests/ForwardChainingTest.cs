using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ForwardChainingTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFactErrorInSecondCondition_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1"};
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredOnce<ForwardChainingSecondRule>();
        }
        
        [Fact]
        public void Fire_OneMatchingFact_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", JoinProperty = null};
            Session.Insert(fact1);

            //Act - Assert
            var ex = Assert.Throws<RuleActionEvaluationException>(() => Session.Fire());
            Assert.NotNull(ex.InnerException);
            Assert.IsType<RuleConditionEvaluationException>(ex.InnerException);
        }

        [Fact]
        public void Fire_OneMatchingFactOfOneKindAndOneOfAnotherKind_FiresSecondRuleDirectlyAndChained()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", JoinProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredTwice<ForwardChainingSecondRule>();
        }

        protected override void SetUpRules()
        {
            SetUpRule<ForwardChainingFirstRule>();
            SetUpRule<ForwardChainingSecondRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class FactType2
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class ForwardChainingFirstRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.Insert(new FactType2
                    {
                        TestProperty = fact1.JoinProperty,
                        JoinProperty = fact1.TestProperty
                    }));
            }
        }

        public class ForwardChainingSecondRule : Rule
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