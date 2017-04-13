using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class CollectionWithConditionsRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_TwoMatchingFacts_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }
        
        [Fact]
        public void Fire_ThreeMatchingFacts_FiresOnceWithThreeFacts()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};
            var fact3 = new FactType {TestProperty = "Valid Value 3"};

            var facts = new[] {fact1, fact2, fact3};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(3, GetFiredFact<IEnumerable<FactType>>().Count());
        }
        
        [Fact]
        public void Fire_ThreeMatchingFactsOneRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType {TestProperty = "Valid Value 1"};
            var fact2 = new FactType {TestProperty = "Valid Value 2"};
            var fact3 = new FactType {TestProperty = "Valid Value 3"};

            var facts = new[] {fact1, fact2};
            Session.InsertAll(facts);
            Session.Insert(fact3);
            Session.Retract(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                IEnumerable<FactType> collection = null;

                When()
                    .Query(() => collection, x => x
                        .Match<FactType>(f => f.TestProperty.StartsWith("Valid"))
                        .Collect()
                        .Where(c => c.Count() > 2));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}