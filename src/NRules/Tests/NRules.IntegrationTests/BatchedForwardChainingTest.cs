using System;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class BatchedForwardChainingTest : BaseRuleTestFixture
    {
        public BatchedForwardChainingTest()
        {
            Session.AutoPropagateLinkedFacts = false;
        }

        [Fact]
        public void Fire_OneMatchingFact_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact1 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.Insert(fact1);

            //Act
            Session.Fire();
            Session.PropagateLinked();

            Session.Fire();

            //Assert
            AssertFiredOnce<ForwardChainingFirstRule>();
            AssertFiredOnce<ForwardChainingSecondRule>();
        }

        [Fact]
        public void Fire_TwoMatchingFacts_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});

            //Act
            Session.Fire();
            var result = Session.PropagateLinked();

            Session.Fire();

            //Assert
            AssertFiredTwice<ForwardChainingFirstRule>();
            AssertFiredTwice<ForwardChainingSecondRule>();
            Assert.Equal(1, result.Count());
            Assert.Equal(LinkedFactAction.Insert, result.ElementAt(0).Action);
            Assert.Equal(2, result.ElementAt(0).FactCount);
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedThenUpdated_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});
            Session.UpdateAll(new []{fact11, fact12});

            //Act
            Session.Fire();
            var result = Session.PropagateLinked();
            Session.Fire();

            //Assert
            AssertFiredTwice<ForwardChainingFirstRule>();
            AssertFiredTwice<ForwardChainingSecondRule>();
            Assert.Equal(1, result.Count());
            Assert.Equal(LinkedFactAction.Insert, result.ElementAt(0).Action);
            Assert.Equal(2, result.ElementAt(0).FactCount);
        }

        [Fact]
        public void Fire_ManyMatchingFactsInsertedPropagatedThenUpdatedTwice_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 2", ChainProperty = "Valid Value 2" };
            var fact13 = new FactType1 { TestProperty = "Valid Value 3", ChainProperty = "Valid Value 3" };
            var fact14 = new FactType1 { TestProperty = "Valid Value 4", ChainProperty = "Valid Value 4" };
            var fact15 = new FactType1 { TestProperty = "Valid Value 5", ChainProperty = "Valid Value 5" };
            var fact16 = new FactType1 { TestProperty = "Valid Value 6", ChainProperty = "Valid Value 6" };
            var fact17 = new FactType1 { TestProperty = "Valid Value 7", ChainProperty = "Valid Value 7" };
            var fact18 = new FactType1 { TestProperty = "Valid Value 8", ChainProperty = "Valid Value 8" };
            var fact19 = new FactType1 { TestProperty = "Valid Value 9", ChainProperty = "Valid Value 9" };
            Session.InsertAll(new []{fact11, fact12, fact13, fact14, fact15, fact16, fact17, fact18, fact19});

            Session.Fire();
            Session.PropagateLinked();

            //Act
            Session.Update(fact11);
            Session.Update(fact12);
            Session.Update(fact13);
            Session.Update(fact14);
            Session.Update(fact15);
            Session.Update(fact16);
            Session.Update(fact17);
            Session.Update(fact18);
            Session.Update(fact19);

            Session.Update(fact11);
            Session.Update(fact12);
            Session.Update(fact13);
            Session.Update(fact14);
            Session.Update(fact15);
            Session.Update(fact16);
            Session.Update(fact17);
            Session.Update(fact18);
            Session.Update(fact19);

            Session.Fire();
            var result = Session.PropagateLinked();

            Session.Fire();

            //Assert
            AssertFiredTimes<ForwardChainingFirstRule>(18);
            AssertFiredTimes<ForwardChainingSecondRule>(18);
            Assert.Equal(1, result.Count());
            Assert.Equal(LinkedFactAction.Update, result.ElementAt(0).Action);
            Assert.Equal(9, result.ElementAt(0).FactCount);
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedThenUpdatedThenRetracted_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});
            Session.UpdateAll(new []{fact11, fact12});
            Session.RetractAll(new []{fact11, fact12});

            //Act
            Session.Fire();
            var result = Session.PropagateLinked();

            Session.Fire();

            //Assert
            AssertDidNotFire<ForwardChainingFirstRule>();
            AssertDidNotFire<ForwardChainingSecondRule>();
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedAndFiredThenUpdatedToInvalidateSecond_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});

            Session.Fire();
            Session.PropagateLinked();

            Session.Fire();

            fact11.ChainProperty = "Invalid Value 1";
            fact12.ChainProperty = "Invalid Value 1";
            Session.UpdateAll(new []{fact11, fact12});

            //Act
            Session.Fire();
            var result = Session.PropagateLinked();
            Session.Fire();

            //Assert
            AssertFiredTimes<ForwardChainingFirstRule>(4);
            AssertFiredTimes<ForwardChainingSecondRule>(2);
            Assert.Equal(1, result.Count());
            Assert.Equal(LinkedFactAction.Retract, result.ElementAt(0).Action);
            Assert.Equal(2, result.ElementAt(0).FactCount);
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedAndFiredThenUpdatedToInvalid_FiresFirstRuleAndChainsSecond()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});

            Session.Fire();
            Session.PropagateLinked();

            Session.Fire();

            fact11.TestProperty = "Invalid Value 1";
            fact12.TestProperty = "Invalid Value 1";
            Session.UpdateAll(new []{fact11, fact12});

            //Act
            Session.Fire();
            var result = Session.PropagateLinked();

            //Assert
            AssertFiredTimes<ForwardChainingFirstRule>(2);
            AssertFiredTimes<ForwardChainingSecondRule>(2);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void Fire_TwoMatchingFactsInsertedAndFiredThenLinkedFactsRetractedAndExceptionThrown_RetractsAndThrows()
        {
            //Arrange
            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});

            Session.Fire();
            Session.PropagateLinked();

            Session.Fire();

            fact11.ChainProperty = "Throw";
            fact12.ChainProperty = "Throw";
            Session.UpdateAll(new []{fact11, fact12});

            //Act
            Assert.Throws<RuleRhsExpressionEvaluationException>(() => Session.Fire());
            Assert.Throws<RuleRhsExpressionEvaluationException>(() => Session.Fire());
            var result = Session.PropagateLinked();

            //Assert
            AssertFiredTimes<ForwardChainingFirstRule>(2);
            AssertFiredTimes<ForwardChainingSecondRule>(2);
            Assert.Equal(1, result.Count());
            Assert.Equal(LinkedFactAction.Retract, result.ElementAt(0).Action);
            Assert.Equal(2, result.ElementAt(0).FactCount);
        }

        [Fact]
        public void FireWithAutoPropagation_TwoMatchingFactsInsertedAndFiredThenLinkedFactsRetractedAndExceptionThrown_RetractsAndThrows()
        {
            //Arrange
            Session.AutoPropagateLinkedFacts = true;

            var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            var fact12 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Valid Value 1" };
            Session.InsertAll(new []{fact11, fact12});

            Session.Fire();

            fact11.ChainProperty = "Throw";
            fact12.ChainProperty = "Throw";
            Session.UpdateAll(new []{fact11, fact12});

            int retracted = 0;
            Session.Events.FactRetractedEvent += (sender, args) => retracted++;

            //Act
            Assert.Throws<RuleRhsExpressionEvaluationException>(() => Session.Fire());
            Assert.Throws<RuleRhsExpressionEvaluationException>(() => Session.Fire());

            //Assert
            AssertFiredTimes<ForwardChainingFirstRule>(2);
            AssertFiredTimes<ForwardChainingSecondRule>(2);
            Assert.Equal(2, retracted);
        }

        protected override void SetUpRules()
        {
            SetUpRule<ForwardChainingFirstRule>();
            SetUpRule<ForwardChainingSecondRule>();
        }

        public class FactType1
        {
            public string TestProperty { get; set; }
            public string ChainProperty { get; set; }
        }

        public class FactType2
        {
            public int UpdateCount { get; set; } = 1;
            public string TestProperty { get; set; }
        }

        public class ForwardChainingFirstRule : Rule
        {
            public override void Define()
            {
                FactType1 fact1 = null;

                When()
                    .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    //.Yield(ctx => Create(fact1), (ctx, fact2) => Update(fact1, fact2))
                    .Do(ctx => YieldIfValid(ctx, fact1));
            }

            private void YieldIfValid(IContext ctx, FactType1 fact1)
            {
                var fact2 = (FactType2)ctx.GetLinked("key");
                if (fact1.ChainProperty.StartsWith("Valid"))
                {
                    if (fact2 == null)
                        ctx.InsertLinked("key", Create(fact1));
                    else
                        ctx.UpdateLinked("key", Update(fact1, fact2));
                }
                else if (fact2 != null)
                {
                    ctx.RetractLinked("key", fact2);
                }

                if (fact1.ChainProperty == "Throw")
                    throw new InvalidOperationException("Chaining failed");
            }

            private static FactType2 Create(FactType1 fact1)
            {
                var fact2 = new FactType2 {TestProperty = fact1.ChainProperty};
                return fact2;
            }

            private static FactType2 Update(FactType1 fact1, FactType2 fact2)
            {
                fact2.TestProperty = fact1.ChainProperty;
                fact2.UpdateCount++;
                return fact2;
            }
        }

        public class ForwardChainingSecondRule : Rule
        {
            public override void Define()
            {
                FactType2 fact2 = null;

                When()
                    .Match(() => fact2, f => f.TestProperty.StartsWith("Valid"));
                Then()
                    .Do(ctx => ctx.NoOp());
            }
        }
    }
}