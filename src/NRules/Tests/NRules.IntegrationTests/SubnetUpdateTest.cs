using System;
using System.Collections.Generic;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class SubnetUpdateTest : BaseRuleTestFixture
    {
        [Fact]
        public void Fire_InsertThenUpdate_OnlyNecessaryEvaluations()
        {
            //Arrange
            var fact11 = new FactType1();
            var fact12 = new FactType1();
            var fact21 = new FactType2();
            var fact22 = new FactType2();

            Session.InsertAll(new[] {fact11, fact12});
            Session.InsertAll(new[] {fact21, fact22});

            int evaluations = 0;
            Session.Events.LhsExpressionEvaluatedEvent += (sender, args) => evaluations++;

            //Act
            Session.UpdateAll(new []{fact11, fact12});
            Session.Fire();

            //Assert
            AssertFiredTwice();
            Assert.Equal(2, evaluations);
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        public class FactType1
        {
        }

        public class FactType2
        {
        }

        public class TestRule : Rule
        {
            public Action<IContext> Action = ctx => { };

            public override void Define()
            {
                FactType1 fact = null;
                IEnumerable<FactType2> collection = null;

                When()
                    .Match(() => fact)
                    .Query(() => collection, x => x
                        .Match<FactType2>()
                        .Select(f => TransformFact(f))
                        .Collect()
                        .Select(c => TransformCollection(c))
                        .Select(c => TransformWithJoin(c, fact))
                    );
                Then()
                    .Do(ctx => Action(ctx));
            }

            private static FactType2 TransformFact(FactType2 fact)
            {
                return fact;
            }

            private IEnumerable<FactType2> TransformCollection(IEnumerable<FactType2> facts)
            {
                return facts;
            }

            private IEnumerable<FactType2> TransformWithJoin(IEnumerable<FactType2> facts, FactType1 fact)
            {
                return facts;
            }
        }
    }
}