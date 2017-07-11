using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactCalculateRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFactOfEachKind_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal("Valid Value 1|Valid Value 2", GetFiredFact<CalculatedFact>().Value);
        }

        [Fact]
        public void Fire_OneMatchingFactOfEachKindSecondFactUpdated_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Valid Value 22";
            Session.Update(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal("Valid Value 1|Valid Value 22", GetFiredFact<CalculatedFact>().Value);
        }

        [Fact]
        public void Fire_OneMatchingFactOfEachKindSecondFactRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1" };
            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
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
            public string JoinProperty { get; set; }
        }

        public class CalculatedFact
        {
            public CalculatedFact(FactType1 fact1, FactType2 fact2)
            {
                Value = $"{fact1.TestProperty}|{fact2.TestProperty}";
            }

            public string Value { get; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;
                FactType2 fact2 = null;
                CalculatedFact fact3 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                    .Calculate(() => fact3, () => new CalculatedFact(fact1, fact2));

                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}
