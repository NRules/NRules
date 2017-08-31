using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactFilterRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_MatchingFacts_Fires()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2" };

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_MatchingFactsInsertAndFireThenUpdateKey1ChangedAndFire_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2" };

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            fact2.TestProperty = "Valid Value 22";
            Session.Update(fact2);
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_MatchingFactsInsertAndFireThenUpdateKey2ChangedAndFire_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2" };

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            fact1.TestProperty = "Valid Value 11";
            Session.Update(fact1);
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_MatchingFactsInsertAndFireThenUpdateKeyDidNotChangeAndFire_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2" };

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            Session.Update(fact2);
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
                FactType1 fact1 = null;
                FactType2 fact2 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"));

                Filter()
                    .OnChange(() => fact1.TestProperty)
                    .OnChange(() => fact2.TestProperty);

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}