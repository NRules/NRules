using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests;

public class BatchedForwardChainingUnlinkedTest : BaseRulesTestFixture
{
    public BatchedForwardChainingUnlinkedTest()
    {
        Session.AutoPropagateLinkedFacts = false;
    }

    [Fact]
    public void Fire_ManyMatchingFactsInsertedUpdatedRetracted_FiresFirstRuleAndChainsSecond()
    {
        //Arrange
        var fact11 = new FactType1 { TestProperty = "Valid Value 1", ChainProperty = "Invalid Value 1" };
        var fact12 = new FactType1 { TestProperty = "Valid Value 2", ChainProperty = "Invalid Value 2" };
        var fact13 = new FactType1 { TestProperty = "Valid Value 3", ChainProperty = "Invalid Value 3" };
        var fact14 = new FactType1 { TestProperty = "Valid Value 4", ChainProperty = "Valid Value 4" };
        var fact15 = new FactType1 { TestProperty = "Valid Value 5", ChainProperty = "Valid Value 5" };
        var fact16 = new FactType1 { TestProperty = "Valid Value 6", ChainProperty = "Valid Value 6" };
        var fact17 = new FactType1 { TestProperty = "Valid Value 7", ChainProperty = "Valid Value 7" };
        var fact18 = new FactType1 { TestProperty = "Valid Value 8", ChainProperty = "Valid Value 8" };
        var fact19 = new FactType1 { TestProperty = "Valid Value 9", ChainProperty = "Valid Value 9" };
        Session.InsertAll(new[] { fact11, fact12, fact13, fact14, fact15, fact16, fact17, fact18, fact19 });

        Session.Fire();
        Session.PropagateChained();

        //Act
        fact11.ChainProperty = "Valid Value 11";
        fact12.ChainProperty = "Valid Value 21";
        fact13.ChainProperty = "Valid Value 31";

        fact14.ChainProperty = "Valid Value 41";
        fact15.ChainProperty = "Valid Value 51";
        fact16.ChainProperty = "Valid Value 61";
        fact17.ChainProperty = "Valid Value 71";

        fact18.ChainProperty = "Invalid Value 81";
        fact19.ChainProperty = "Invalid Value 91";
        Session.UpdateAll(new[] { fact11, fact12, fact13, fact14, fact15, fact16, fact17, fact18, fact19 });

        Session.Fire();
        var result = Session.PropagateChained();

        Session.Fire();

        //Assert
        Verify.Rule<ForwardChainingFirstRule>().FiredTimes(18);
        Verify.Rule<ForwardChainingSecondRule>().FiredTimes(13);
        Assert.Equal(3, result.Count());
        Assert.Equal(ChainedFactAction.Update, result.ElementAt(0).Action);
        Assert.Equal(4, result.ElementAt(0).FactCount);
        Assert.Equal(ChainedFactAction.Retract, result.ElementAt(1).Action);
        Assert.Equal(2, result.ElementAt(1).FactCount);
        Assert.Equal(ChainedFactAction.Insert, result.ElementAt(2).Action);
        Assert.Equal(3, result.ElementAt(2).FactCount);
    }

    protected override void SetUpRules(Testing.IRepositorySetup setup)
    {
        setup.Rule<ForwardChainingFirstRule>();
        setup.Rule<ForwardChainingSecondRule>();
    }

    public class FactType1
    {
        public string TestProperty { get; set; }
        public string ChainProperty { get; set; }

        public override string ToString()
        {
            return $"FactType1: TestProperty={TestProperty} ChainProperty={ChainProperty}";
        }
    }

    public class FactType2
    {
        public FactType1 Parent { get; set; }
        public int UpdateCount { get; set; } = 1;
        public string TestProperty { get; set; }

        public override string ToString()
        {
            return $"FactType2: TestProperty={TestProperty} UpdateCount={UpdateCount}";
        }
    }

    public class ForwardChainingFirstRule : Rule
    {
        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType2> chained = null;

            When()
                .Match(() => fact1, f => f.TestProperty.StartsWith("Valid"))
                .Query(() => chained, q => q
                    .Match<FactType2>(f => f.Parent == fact1)
                    .Collect());

            Filter()
                .OnChange(() => fact1.TestProperty, () => fact1.ChainProperty);

            Then()
                .Do(ctx => YieldIfValid(ctx, fact1, chained.FirstOrDefault()));
        }

        private void YieldIfValid(IContext ctx, FactType1 fact1, FactType2 fact2)
        {
            if (fact1.ChainProperty.StartsWith("Valid"))
            {
                if (fact2 == null)
                    ctx.QueueInsert(Create(fact1));
                else
                    ctx.QueueUpdate(Update(fact1, fact2));
            }
            else if (fact2 != null)
            {
                ctx.QueueRetract(fact2);
            }

            if (fact1.ChainProperty == "Throw")
                throw new InvalidOperationException("Chaining failed");
        }

        private static FactType2 Create(FactType1 fact1)
        {
            var fact2 = new FactType2 { Parent = fact1, TestProperty = fact1.ChainProperty };
            return fact2;
        }

        private static FactType2 Update(FactType1 fact1, FactType2 fact2)
        {
            fact2.Parent = fact1;
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