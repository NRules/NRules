using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class TwoFactOneJoinedGroupByRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnother_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11"};
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = null};
            var fact4 = new FactType2 {TestProperty = "Invalid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};
            var fact5 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 23", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            var facts = new[] {fact2, fact3, fact4, fact5};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(0));
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(1));
        }

        [Fact]
        public void Fire_OneMatchingFactOfOneKindAndNoneOfAnother_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value", GroupKey = "Group 11" };

            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherInsertedInOppositeOrder_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = null};
            var fact4 = new FactType2 {TestProperty = "Invalid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};
            var fact5 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 23", JoinProperty = fact1.TestProperty};

            var facts = new[] {fact2, fact3, fact4, fact5};
            Session.InsertAll(facts);
            Session.Insert(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(0));
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(1));
        }

        [Fact]
        public void Fire_OneMatchingFactOfOneKindAndTwoOfAnotherThenFireThenAnotherMatchingFactForSecondGroupThenFire_FiresTwiceWithOneFactInEachGroupThenFiresAgainWithTwoFactsInOneGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};
            var fact23 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act
            Session.Fire();
            var actualCount11 = GetFiredFact<IGrouping<string, GroupElement>>(0).Count();
            var actualCount12 = GetFiredFact<IGrouping<string, GroupElement>>(1).Count();

            Session.Insert(fact23);
            Session.Fire();
            var actualCount2 = GetFiredFact<IGrouping<string, GroupElement>>(2).Count();

            //Assert
            AssertFiredTimes(3);
            Assert.Equal(1, actualCount11);
            Assert.Equal(1, actualCount12);
            Assert.Equal(2, actualCount2);
        }

        [Fact]
        public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenOneRetracted_FiresOnceWithOneFactInGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            Session.Retract(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>());
        }

        [Fact]
        public void Fire_FactOfOneKindIsValidAndTwoOfAnotherKindAreAssertedThenRetracted_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            Session.Insert(fact2);
            Session.Insert(fact3);

            Session.Retract(fact2);
            Session.Retract(fact3);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FactOfOneKindIsInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            var facts = new[] {fact2, fact3, fact4};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FactOfOneKindIsAssertedThenRetractedAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            var facts = new[] {fact2, fact3, fact4};
            Session.InsertAll(facts);

            Session.Retract(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FactOfOneKindIsAssertedThenUpdatedToInvalidAndTwoOfAnotherKindAreValid_DoesNotFire()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = fact1.TestProperty};
            var fact4 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact1.TestProperty};

            Session.Insert(fact1);
            var facts = new[] {fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact1.TestProperty = "Invalid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FactOfOneKindIsInvalidThenUpdatedToValidAndTwoOfAnotherKindAreValid_FiresTwiceWithOneFactInEachGroup()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Invalid Value 1", GroupKey = "Group 11" };
            var fact2 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = "Valid Value 1"};
            var fact3 = new FactType2 {TestProperty = "Invalid Value", GroupKey = "Group 21", JoinProperty = "Valid Value 1"};
            var fact4 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = "Valid Value 1"};

            Session.Insert(fact1);
            var facts = new[] {fact2, fact3, fact4};
            Session.InsertAll(facts);

            fact1.TestProperty = "Valid Value 1";
            Session.Update(fact1);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(0));
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(1));
        }

        [Fact]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingOneOfTheFacts_FiresOnceWithTwoFactsInGroups()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 1" };
            var fact12 = new FactType1 {TestProperty = "Valid Value 2", GroupKey = "Group 1" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal(2, GetFiredFact<IGrouping<string, GroupElement>>().Count());
        }

        [Fact]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFacts_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11" };
            var fact12 = new FactType1 {TestProperty = "Valid Value 2", GroupKey = "Group 12" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact23 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact11.TestProperty};
            var fact24 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            var facts = new[] {fact21, fact22, fact23, fact24};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            var firedFacts2 = new[]
            {
                GetFiredFact<IGrouping<string, GroupElement>>(0),
                GetFiredFact<IGrouping<string, GroupElement>>(1),
                GetFiredFact<IGrouping<string, GroupElement>>(2)
            };
            var firedFacts1 = new[] {GetFiredFact<FactType1>(0), GetFiredFact<FactType1>(1), GetFiredFact<FactType1>(2)};
            var validAmountsPerGroup = firedFacts2.Count(x => x.Count() == 1) == 2 &&
                                       firedFacts2.Count(x => x.Count() == 2) == 1;
            var valid1 = firedFacts1.Count(x => Equals(fact11, x)) == 2;
            var valid2 = firedFacts1.Count(x => Equals(fact12, x)) == 1;
            Assert.True(validAmountsPerGroup && valid1 && valid2);
        }

        [Fact]
        public void Fire_BulkInsertForMultipleTypes_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2", GroupKey = "Group 12" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact23 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact11.TestProperty};
            var fact24 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact12.TestProperty};

            var facts = new object[] {fact11, fact12, fact21, fact22, fact23, fact24};
            Session.InsertAll(facts);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            var firedFacts2 = new[]
            {
                GetFiredFact<IGrouping<string, GroupElement>>(0),
                GetFiredFact<IGrouping<string, GroupElement>>(1),
                GetFiredFact<IGrouping<string, GroupElement>>(2)
            };
            var firedFacts1 = new[] {GetFiredFact<FactType1>(0), GetFiredFact<FactType1>(1), GetFiredFact<FactType1>(2)};
            var validAmountsPerGroup = firedFacts2.Count(x => x.Count() == 1) == 2 &&
                                       firedFacts2.Count(x => x.Count() == 2) == 1;
            var valid1 = firedFacts1.Count(x => Equals(fact11, x)) == 2;
            var valid2 = firedFacts1.Count(x => Equals(fact12, x)) == 1;
            Assert.True(validAmountsPerGroup && valid1 && valid2);
        }

        [Fact]
        public void Fire_TwoFactsOfOneKindAndAggregatedFactsMatchingBothOfTheFactsInsertInReverse_FiresThreeTimesWithCorrectCounts()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2", GroupKey = "Group 11" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact23 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 22", JoinProperty = fact11.TestProperty};
            var fact24 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty};

            var facts = new[] {fact24, fact23, fact22, fact21};
            Session.InsertAll(facts);
            var facts2 = new[] {fact12, fact11};
            Session.InsertAll(facts2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTimes(3);
            var firedFacts2 = new[]
            {
                GetFiredFact<IGrouping<string, GroupElement>>(0),
                GetFiredFact<IGrouping<string, GroupElement>>(1),
                GetFiredFact<IGrouping<string, GroupElement>>(2)
            };
            var firedFacts1 = new[] {GetFiredFact<FactType1>(0), GetFiredFact<FactType1>(1), GetFiredFact<FactType1>(2)};
            var validAmountsPerGroup = firedFacts2.Count(x => x.Count() == 1) == 2 &&
                                       firedFacts2.Count(x => x.Count() == 2) == 1;
            var valid1 = firedFacts1.Count(x => Equals(fact11, x)) == 2;
            var valid2 = firedFacts1.Count(x => Equals(fact12, x)) == 1;
            Assert.True(validAmountsPerGroup && valid1 && valid2);
        }

        [Fact]
        public void Fire_TwoMatchingCombinationsThenOneFactOfFirstKindUpdated_FiresTwiceBeforeUpdateAndOnceAfter()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2", GroupKey = "Group 12" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredTimes(2);

            //Act - 2
            Session.Update(fact11);
            Session.Fire();

            //Assert - 2
            AssertFiredTimes(3);
        }

        [Fact]
        public void Fire_TwoMatchingCombinationsThenOneFactOfSecondKindUpdated_FiresTwiceBeforeUpdateAndOnceAfter()
        {
            //Arrange
            var fact11 = new FactType1 {TestProperty = "Valid Value 1", GroupKey = "Group 11"};
            var fact12 = new FactType1 {TestProperty = "Valid Value 2", GroupKey = "Group 12" };
            var fact21 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact11.TestProperty};
            var fact22 = new FactType2 {TestProperty = "Valid Value", GroupKey = "Group 21", JoinProperty = fact12.TestProperty};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact22);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredTimes(2);

            //Act - 2
            Session.Update(fact21);
            Session.Fire();

            //Assert - 2
            AssertFiredTimes(3);
        }

        [Fact]
        public void Fire_OneFactOfOneKindAndAggregatedFactsMatchThenFactUpdatedWithDifferentGroupKey_FiresOnceFactsInNewGroup()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 1" };
            var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
            var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };

            Session.Insert(fact11);
            Session.Insert(fact21);
            Session.Insert(fact22);

            fact11.GroupKey = "Group 2";
            Session.Update(fact11);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
            Assert.Equal("Group 2|Group 1", GetFiredFact<IGrouping<string, GroupElement>>().Key);
        }

        [Fact]
        public void Fire_OneFactOfOneKindAndAggregatedFactsMatchThenAggregatedFactUpdatedWithDifferentGroupKey_FiresTwiceFactsInCorrectGroups()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", GroupKey = "Group 1" };
            var fact21 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
            var fact22 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 1", JoinProperty = fact11.TestProperty };
            var fact23 = new FactType2 { TestProperty = "Valid Value", GroupKey = "Group 2", JoinProperty = fact11.TestProperty };

            Session.Insert(fact11);
            Session.Insert(fact21);
            Session.Insert(fact22);
            Session.Insert(fact23);

            fact22.GroupKey = "Group 2";
            Session.Update(fact22);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal("Group 1|Group 1", GetFiredFact<IGrouping<string, GroupElement>>(0).Key);
            Assert.Single(GetFiredFact<IGrouping<string, GroupElement>>(0));
            Assert.Equal("Group 1|Group 2", GetFiredFact<IGrouping<string, GroupElement>>(1).Key);
            Assert.Equal(2, GetFiredFact<IGrouping<string, GroupElement>>(1).Count());
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string GroupKey { get; set; }
            public string TestProperty { get; set; }
        }

        public class FactType2
        {
            public string GroupKey { get; set; }
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
        }

        public class GroupElement
        {
            public GroupElement(FactType1 fact1, FactType2 fact2)
            {
                TestProperty = $"{fact1.TestProperty}|{fact2.TestProperty}";
            }

            public string TestProperty { get; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType1 fact = null;
                IGrouping<string, GroupElement> group = null;

                When()
                    .Match<FactType1>(() => fact, f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => group, x => x
                        .Match<FactType2>(
                            f => f.TestProperty.StartsWith("Valid"),
                            f => f.JoinProperty == fact.TestProperty)
                        .GroupBy(f => GetKey(fact, f), f => new GroupElement(fact, f)));
                Then()
                    .Do(ctx => ctx.NoOp());
            }

            private static string GetKey(FactType1 fact1, FactType2 fact2)
            {
                return $"{fact1.GroupKey}|{fact2.GroupKey}";
            }
        }
    }
}
