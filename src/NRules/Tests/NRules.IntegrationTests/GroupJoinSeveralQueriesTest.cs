using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class GroupJoinSeveralQueriesTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_TwoMatchingFactsOfOneKindAndJoinedGroups_FiresTwice()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 11", JoinProperty = "Group 1"};
            var fact12 = new FactType1 { TestProperty = "Valid Value 12", JoinProperty = "Group 2"};
            var fact21 = new FactType2(1) { TestProperty = "Value 1" };
            var fact31 = new FactType3() { TestProperty = "Valid Value 31", GroupProperty = "Group 1"};
            var fact32 = new FactType3() { TestProperty = "Valid Value 32", GroupProperty = "Group 2"};

            Session.Insert(fact11);
            Session.Insert(fact12);
            Session.Insert(fact21);
            Session.Insert(fact31);
            Session.Insert(fact32);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, fact11.EvalCount);
        }

        [Fact]
        public void Fire_MatchingSetFactOfFirstKindUpdated_FiresOnceThenFiresOnceAgain()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 11", JoinProperty = "Group 1"};
            var fact21 = new FactType2(1) { TestProperty = "Value 1" };
            var fact31 = new FactType3() { TestProperty = "Valid Value 31", GroupProperty = "Group 1"};

            Session.Insert(fact11);
            Session.Insert(fact21);
            Session.Insert(fact31);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredOnce();
            Assert.Equal(1, fact11.EvalCount);

            //Act - 2
            Session.Update(fact11);
            Session.Fire();

            //Assert - 2
            AssertFiredTwice();
            Assert.Equal(2, fact11.EvalCount);
        }

        [Fact]
        public void Fire_MatchingSetFactOfSecondKindUpdated_FiresOnceThenFiresOnceAgain()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 11", JoinProperty = "Group 1"};
            var fact21 = new FactType2(1) { TestProperty = "Value 1" };
            var fact31 = new FactType3() { TestProperty = "Valid Value 31", GroupProperty = "Group 1"};

            Session.Insert(fact11);
            Session.Insert(fact21);
            Session.Insert(fact31);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredOnce();
            Assert.Equal(1, fact11.EvalCount);

            //Act - 2
            Session.Update(fact21);
            Session.Fire();

            //Assert - 2
            AssertFiredTwice();
            Assert.Equal(2, fact11.EvalCount);
        }

        [Fact]
        public void Fire_MatchingSetFactOfThirdKindUpdated_FiresOnceThenFiresOnceAgain()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 11", JoinProperty = "Group 1"};
            var fact21 = new FactType2(1) { TestProperty = "Value 1" };
            var fact31 = new FactType3() { TestProperty = "Valid Value 31", GroupProperty = "Group 1"};

            Session.Insert(fact11);
            Session.Insert(fact21);
            Session.Insert(fact31);

            //Act - 1
            Session.Fire();

            //Assert - 1
            AssertFiredOnce();
            Assert.Equal(1, fact11.EvalCount);

            //Act - 2
            Session.Update(fact31);
            Session.Fire();

            //Assert - 2
            AssertFiredTwice();
            Assert.Equal(2, fact11.EvalCount);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
            public string JoinProperty { get; set; }
            public int EvalCount { get; set; }
        }

        public class FactType2
        {
            public FactType2(int id)
            {
                Id = id;
            }

            public int Id { get; private set; }
            public string TestProperty { get; set; }
        }

        public class FactType3
        {
            public string TestProperty { get; set; }
            public string GroupProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;
                FactType2 fact21 = null;
                FactType2 fact22 = null;
                IEnumerable<FactType3> group = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => fact21, q => q
                        .Match<FactType2>(f => f.TestProperty == "Value 1")
                        .Collect()
                        .Select(x => x.FirstOrDefault() ?? new FactType2(0) {TestProperty = "Value 1"}))
                    .Query(() => fact22, q => q
                        .Match<FactType2>(f => f.TestProperty == "Value 2")
                        .Collect()
                        .Select(x => x.FirstOrDefault() ?? new FactType2(0) {TestProperty = "Value 2"}))
                    .Query(() => group, q => q
                        .Match<FactType3>(f => f.TestProperty.StartsWith("Valid"))
                        .GroupBy(x => x.GroupProperty)
                        .Where(g => ValidGroup(fact1, fact21, fact22, g)));
                Then()
                    .Do(ctx => ctx.NoOp());
            }

            private bool ValidGroup(FactType1 fact1, FactType2 fact21, FactType2 fact22, IGrouping<string, FactType3> group)
            {
                fact1.EvalCount++;
                return group.Key == fact1.JoinProperty;
            }
        }
    }
}