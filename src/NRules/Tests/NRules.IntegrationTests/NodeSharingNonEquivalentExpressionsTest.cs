using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class NodeSharingNonEquivalentExpressionsTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_FirstRuleMatchesFacts_OnlyMatchingRuleFires()
    {
        //Arrange
        var thisNameFact = new NameFact { Name = "ThisName" };
        var otherNameFact = new NameFact { Name = "OtherName" };
        var referenceFact = new ReferenceFact { Reference = thisNameFact };

        Session.Insert(thisNameFact);
        Session.Insert(otherNameFact);
        Session.Insert(referenceFact);

        //Act
        Session.Fire();

        //Assert
        Verify(x =>
        {
            x.Rule<TestRule1>().Fired();
            x.Rule<TestRule3>().Fired();
        });
        Verify(x => x.Rule<TestRule2>().Fired(Times.Never));
    }

    [Fact]
    public void Fire_SecondRuleMatchesFacts_OnlyMatchingRuleFires()
    {
        //Arrange
        var thisNameFact = new NameFact { Name = "ThisName" };
        var otherNameFact = new NameFact { Name = "OtherName" };
        var referenceFact = new ReferenceFact { Reference = otherNameFact };

        Session.Insert(thisNameFact);
        Session.Insert(otherNameFact);
        Session.Insert(referenceFact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule<TestRule1>().Fired(Times.Never));
        Verify(x =>
        {
            x.Rule<TestRule2>().Fired();
            x.Rule<TestRule4>().Fired();
        });
    }

    [Fact]
    public void Fire_ThirdRuleMatchesFacts_OnlyMatchingRuleFires()
    {
        //Arrange
        var thisNameFact = new NameFact { Name = "ThisName" };
        var otherNameFact = new NameFact { Name = "OtherName" };
        var referenceFact = new ReferenceFact { Reference = thisNameFact };

        Session.Insert(thisNameFact);
        Session.Insert(otherNameFact);
        Session.Insert(referenceFact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule<TestRule4>().Fired(Times.Never));
        Verify(x =>
        {
            x.Rule<TestRule1>().Fired();
            x.Rule<TestRule3>().Fired();
        });
    }

    [Fact]
    public void Fire_FourthRuleMatchesFacts_OnlyMatchingRuleFires()
    {
        //Arrange
        var thisNameFact = new NameFact { Name = "ThisName" };
        var otherNameFact = new NameFact { Name = "OtherName" };
        var referenceFact = new ReferenceFact { Reference = otherNameFact };

        Session.Insert(thisNameFact);
        Session.Insert(otherNameFact);
        Session.Insert(referenceFact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule<TestRule3>().Fired(Times.Never));
        Verify(x =>
        {
            x.Rule<TestRule2>().Fired();
            x.Rule<TestRule4>().Fired();
        });
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        setup.Rule<TestRule1>();
        setup.Rule<TestRule2>();
        setup.Rule<TestRule3>();
        setup.Rule<TestRule4>();
    }

    public class NameFact
    {
        [NotNull]
        public string? Name { get; set; }
    }

    public class ReferenceFact
    {
        [NotNull]
        public NameFact? Reference { get; set; }

        public NameFact GetKey(NameFact nameFact)
        {
            return nameFact;
        }
    }

    public class TestRule1 : Rule
    {
        public override void Define()
        {
            NameFact thisNameFact = null!;
            NameFact otherNameFact = null!;
            ReferenceFact referenceFact = null!;

            When()
                .Match(() => thisNameFact, f => f.Name == "ThisName")
                .Match(() => otherNameFact, f => f.Name == "OtherName")
                .Match(() => referenceFact, rf => rf.Reference == thisNameFact);

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }

    public class TestRule2 : Rule
    {
        public override void Define()
        {
            NameFact thisNameFact = null!;
            NameFact otherNameFact = null!;
            ReferenceFact referenceFact = null!;

            When()
                .Match(() => thisNameFact, f => f.Name == "ThisName")
                .Match(() => otherNameFact, f => f.Name == "OtherName")
                .Match(() => referenceFact, rf => rf.Reference == otherNameFact);

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }

    public class TestRule3 : Rule
    {
        public override void Define()
        {
            NameFact thisNameFact = null!;
            NameFact otherNameFact = null!;
            IEnumerable<ReferenceFact> referenceFacts = null!;

            When()
                .Match(() => thisNameFact, f => f.Name == "ThisName")
                .Match(() => otherNameFact, f => f.Name == "OtherName")
                .Query(() => referenceFacts, q => q
                    .Match<ReferenceFact>()
                    .GroupBy(x => x.GetKey(thisNameFact))
                    .Where(g => g.All(x => x.Reference == g.Key)));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }

    public class TestRule4 : Rule
    {
        public override void Define()
        {
            NameFact thisNameFact = null!;
            NameFact otherNameFact = null!;
            IEnumerable<ReferenceFact> referenceFacts = null!;

            When()
                .Match(() => thisNameFact, f => f.Name == "ThisName")
                .Match(() => otherNameFact, f => f.Name == "OtherName")
                .Query(() => referenceFacts, q => q
                    .Match<ReferenceFact>()
                    .GroupBy(x => x.GetKey(otherNameFact))
                    .Where(g => g.All(x => x.Reference == g.Key)));

            Then()
                .Do(ctx => ctx.NoOp());
        }
    }
}
