using System;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactOneSelectRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFactOfEachKind_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = "Valid Value 1"};
            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal($"{fact1.TestProperty}|{fact2.TestProperty}", GetFiredFact<FactProjection>().Value);
        }

        [Fact]
        public void Fire_TwoPairsOfMatchingFacts_FiresTwice()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 11"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 12"};
            var fact21 = new FactType2 {TestProperty = "Valid Value 21", JoinProperty = "Valid Value 11"};
            var fact22 = new FactType2 {TestProperty = "Valid Value 22", JoinProperty = "Valid Value 12"};
            Session.InsertAll(new[] {fact11, fact12});
            Session.InsertAll(new[] {fact21, fact22});

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal($"{fact11.TestProperty}|{fact21.TestProperty}", GetFiredFact<FactProjection>(0).Value);
            Assert.Equal($"{fact12.TestProperty}|{fact22.TestProperty}", GetFiredFact<FactProjection>(1).Value);
        }

        [Fact]
        public void Fire_MatchingFactOfFirstKind_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_MatchingFactOfSecondKind_DoesNotFire()
        {
            //Arrange
            var fact2 = new FactType2 {TestProperty = "Valid Value 2"};
            Session.Insert(fact2);

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

        public class FactProjection : IEquatable<FactProjection>
        {
            public FactProjection(FactType1 fact1, FactType2 fact2)
            {
                Value = $"{fact1.TestProperty}|{fact2.TestProperty}";
            }

            public string Value { get; }

            public bool Equals(FactProjection other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return string.Equals(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((FactProjection)obj);
            }

            public override int GetHashCode()
            {
                return (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;
                FactProjection projection = null;

                When()
                    .Match(() => fact1)
                    .Query(() => projection, q => q
                        .Match<FactType2>(f => f.JoinProperty == fact1.TestProperty)
                        .Select(f => new FactProjection(fact1, f))
                        .Where(p => p.Value.StartsWith("Valid")));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}