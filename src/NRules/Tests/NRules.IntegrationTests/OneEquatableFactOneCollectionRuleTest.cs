using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class OneEquatableFactOneCollectionRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_TwoMatchingFactsAndOneInvalid_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact2 = new FactType(2) {TestProperty = "Valid Value 2"};
            var fact3 = new FactType(3) {TestProperty = "Invalid Value 3"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }
        
        [Fact]
        public void Fire_OneMatchingFactInsertedThenUpdated_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact11 = new FactType(1) {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);
            Session.Update(fact11);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdated_FiresOnceWithTwoFactsInCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact2 = new FactType(2) {TestProperty = "Valid Value 2"};
            var fact21 = new FactType(2) {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Update(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneRetracted_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact2 = new FactType(2) {TestProperty = "Valid Value 2"};
            var fact21 = new FactType(2) {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Retract(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedTwoRetracted_FiresOnceWithEmptyCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact11 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact2 = new FactType(2) {TestProperty = "Valid Value 2"};
            var fact21 = new FactType(2) {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Retract(fact11);
            Session.Retract(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(0, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedOneUpdatedToInvalid_FiresOnceWithOneFactInCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact2 = new FactType(2) {TestProperty = "Valid Value 2"};
            var fact21 = new FactType(2) {TestProperty = "Invalid Value"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            Session.Update(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(1, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        [Fact]
        public void Fire_OneMatchingFactsAndOneInvalidInsertedTheInvalidUpdatedToValid_FiresOnceWithTwoFactInCollection()
        {
            //Arrange
            var fact1 = new FactType(1) {TestProperty = "Valid Value 1"};
            var fact2 = new FactType(2) {TestProperty = "Invalid Value"};
            var fact21 = new FactType(2) {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            Session.Update(fact21);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IEnumerable<FactType>>().Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType : IEquatable<FactType>
        {
            public FactType(int id)
            {
                Id = id;
            }

            public int Id { get; private set; }
            public string TestProperty { get; set; }
            public string ValueProperty { get; set; }

            public bool Equals(FactType other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Id == other.Id;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((FactType)obj);
            }

            public override int GetHashCode()
            {
                return Id;
            }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IEnumerable<FactType> collection = null;

                When()
                    .Query(() => collection, q => q
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .Collect());
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}