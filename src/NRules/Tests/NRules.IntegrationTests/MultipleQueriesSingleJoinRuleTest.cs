using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using Xunit;

namespace NRules.IntegrationTests
{
    public class MultipleQueriesSingleJoinRuleTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_OneMatchingFactSet_FiresOnce()
        {
            //Arrange
            var fact1 = new FactType1 {TestProperty = "Valid Value 1"};
            var fact4 = new FactType4 {TestProperty = "Valid Value 1"};

            Session.Insert(fact1);
            Session.Insert(fact4);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Fact]
        public void Fire_OneMatchingFactSetOneNotMatching_FiresOnce()
        {
            //Arrange
            var fact1A = new FactType1 {TestProperty = "Valid Value 1"};
            var fact1B = new FactType1 {TestProperty = "Valid Value 2"};
            var fact4A = new FactType4 {TestProperty = "Valid Value 1"};
            var fact4B = new FactType4 {TestProperty = "Valid Value 3"};

            Session.Insert(fact1A);
            Session.Insert(fact1B);
            Session.Insert(fact4A);
            Session.Insert(fact4B);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }
        
        [Fact]
        public void Fire_TwoMatchingFactSets_FiresTwice()
        {
            //Arrange
            var fact1A = new FactType1 {TestProperty = "Valid Value 1"};
            var fact1B = new FactType1 {TestProperty = "Valid Value 2"};
            var fact4A = new FactType4 {TestProperty = "Valid Value 1"};
            var fact4B = new FactType4 {TestProperty = "Valid Value 2"};

            Session.Insert(fact1A);
            Session.Insert(fact1B);
            Session.Insert(fact4A);
            Session.Insert(fact4B);

            //Act
            Session.Fire();

            //Assert
            AssertFiredTwice();
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

        public class FactType4
        {
            public string TestProperty { get; set; }
            public FactType4 Parent { get; set; }
        }

        public class TestRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;
                IEnumerable<FactType2> collection2 = null;
                IEnumerable<FactType3> collection3 = null;
                IEnumerable<FactType4> collection4 = null;

                When()
                    .Match<FactType1>(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                    .Query(() => collection2, q => q
                        .Match<FactType2>(f => f.TestProperty.StartsWith("Valid"))
                        .Collect())
                    .Query(() => collection3, q => q
                        .Match<FactType3>(f => f.TestProperty.StartsWith("Valid"))
                        .Collect())
                    .Query(() => collection4, q => q
                        .Match<FactType4>(f => f.TestProperty.StartsWith("Valid"))
                        .Collect()
                        .Where(x => IsMatch(fact1, collection2, collection3, x)));
                Then()
                    .Do(ctx => ctx.NoOp());
            }

            private bool IsMatch(FactType1 fact1, IEnumerable<FactType2> collection2, IEnumerable<FactType3> collection3, IEnumerable<FactType4> collection4)
            {
                return collection4.Any(x => x.TestProperty == fact1.TestProperty);
            }
        }
    }
}