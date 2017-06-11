using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class ThreeFactOrGroupRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_MatchingMainFactAndNoneOfOrGroup_DoesNotFire()
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
        public void Fire_MatchingMainFactAndFirstPartOfOrGroup_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_MatchingFactsFirstPart_FactsInContext()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            IFactMatch[] matches = null;
            GetRuleInstance<TestRule>().Action = ctx =>
            {
                matches = ctx.Facts.ToArray();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(3, matches.Length);
            Assert.Equal("fact1", matches[0].Declaration.Name);
            Assert.Same(fact1, matches[0].Value);
            Assert.Equal("fact2", matches[1].Declaration.Name);
            Assert.Same(fact2, matches[1].Value);
            Assert.Equal("fact3", matches[2].Declaration.Name);
            Assert.Null(matches[2].Value);
        }

        [Fact]
        public void Fire_MatchingFactsSecondPart_FactsInContext()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1" };
            var fact2 = new FactType2 { TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty };
            var fact3 = new FactType3 { TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            IFactMatch[] matches = null;
            GetRuleInstance<TestRule>().Action = ctx =>
            {
                matches = ctx.Facts.ToArray();
            };

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(3, matches.Length);
            Assert.Equal("fact1", matches[0].Declaration.Name);
            Assert.Same(fact1, matches[0].Value);
            Assert.Equal("fact2", matches[1].Declaration.Name);
            Assert.Same(fact2, matches[1].Value);
            Assert.Equal("fact3", matches[2].Declaration.Name);
            Assert.Same(fact3, matches[2].Value);
        }

        [Fact]
        public void Fire_MatchingMainFactAndSecondPartOfOrGroup_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 { TestProperty = "Invalid Value 2", JoinProperty = fact1.TestProperty };
            var fact3 = new FactType3 { TestProperty = "Valid Value 3", JoinProperty = fact1.TestProperty };

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_MatchingMainFactAndBothPartsOfOrGroup_FiresTwice()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value 3", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType3 {TestProperty = "Valid Value 4", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
        }

        [Fact]
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndMainFactRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Retract(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndMainFactUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact1.TestProperty = "Invalid Value";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndGroupFactRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            Session.Retract(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_MatchingMainFactAndOnePartOfOrGroupAndGroupFactUpdatedToInvalid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact2 = new FactType2 {TestProperty = "Valid Value 2", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);

            fact2.TestProperty = "Invalid Value";
            Session.Update(fact2);

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

        public class FactType3
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public Action<IContext> Action = ctx => { };

            public override void Define()
            {
                FactType1 fact1 = null;
                FactType2 fact2 = null;
                FactType3 fact3 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Or(x => x
                        .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)
                        .And(xx => xx
                            .Match<FactType2>(() => fact2, f => f.TestProperty.StartsWith("Invalid"), f => f.JoinProperty == fact1.TestProperty)
                            .Match<FactType3>(() => fact3, f => f.TestProperty.StartsWith("Valid"), f => f.JoinProperty == fact1.TestProperty)));

                Then()
                    .Do(ctx => Action(ctx));
            }
        }
    }
}