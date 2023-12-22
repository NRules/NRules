﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class AggregateEvaluationExceptionTest : BaseRulesTestFixture
{
    [Fact]
    public void Fire_ErrorInAggregateNoErrorHandler_Throws()
    {
        //Arrange
        _testRule.Grouping = ThrowGrouping;

        Expression expression = null;
        IList<IFact> facts = null;
        Session.Events.LhsExpressionFailedEvent += (sender, args) => expression = args.Expression;
        Session.Events.LhsExpressionFailedEvent += (sender, args) => facts = args.Facts.ToList();

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };

        //Act - Assert
        var ex = Assert.Throws<RuleLhsExpressionEvaluationException>(() => Session.Insert(fact21));
        Assert.NotNull(expression);
        Assert.Equal(2, facts.Count);
        Assert.Same(fact11, facts.First().Value);
        Assert.Same(fact21, facts.Skip(1).First().Value);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void Fire_FailedAssert_DoesNotFire()
    {
        //Arrange
        _testRule.Grouping = ThrowGrouping;

        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_AssertThenFailedAssertForSameJoin_DoesNotFire()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = ThrowGrouping;

        var fact22 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FailedAssertThenAssertForSameJoin_Fires()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        var fact22 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(g => g.Count() == 2)));
    }

    [Fact]
    public void Fire_FailedAssertThenAssertForSameJoinThenUpdate_Fires()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        var fact22 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact22);

        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(g => g.Count() == 2)));
    }

    [Fact]
    public void Fire_AssertThenFailedAssertForDifferentJoin_FiresForSuccessfulJoin()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        Session.Insert(fact12);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = ThrowGrouping;

        var fact22 = new FactType2 { GroupProperty = "Group2", JoinProperty = "Valid Value 2" };
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactType1>(f => f.TestProperty == "Valid Value 1")));
    }

    [Fact]
    public void Fire_FailedAssertThenAssertForDifferentJoin_FiresForSuccessfulJoin()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact12 = new FactType1 { TestProperty = "Valid Value 2" };
        Session.Insert(fact12);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        var fact22 = new FactType2 { GroupProperty = "Group2", JoinProperty = "Valid Value 2" };
        Session.Insert(fact22);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<FactType1>(f => f.TestProperty == "Valid Value 2")));
    }

    [Fact]
    public void Fire_AssertThenFailedUpdate_DoesNotFire()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = ThrowGrouping;
        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_AssertThenFailedUpdateThenUpdate_Fires()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = ThrowGrouping;
        Session.Update(fact21);

        _testRule.Grouping = x => x.GroupProperty;
        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_FailedAssertThenUpdate_Fires()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        Session.Update(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_FailedAssertThenUpdateJoin_Fires()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = ThrowGrouping;

        var fact22 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact22);

        _testRule.Grouping = x => x.GroupProperty;

        Session.Update(fact11);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Matched.Fact<IEnumerable<FactType2>>(g => g.Count() == 2)));
    }

    [Fact]
    public void Fire_FailedAssertThenUpdateThenRetract_DoesNotFire()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        Session.Update(fact21);
        Session.Retract(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FailedAssertThenRetract_DoesNotFire()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        Session.Retract(fact21);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FailedAssertThenRetractJoined_DoesNotFire()
    {
        //Arrange
        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact11 = new FactType1 { TestProperty = "Valid Value 1" };
        Session.Insert(fact11);

        _testRule.Grouping = ThrowGrouping;

        var fact21 = new FactType2 { GroupProperty = "Group1", JoinProperty = "Valid Value 1" };
        Session.Insert(fact21);

        _testRule.Grouping = x => x.GroupProperty;

        Session.Retract(fact11);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        _testRule = new TestRule();
        setup.Rule(_testRule);
    }

    private static readonly Func<FactType2, object> SuccessfulGrouping = x => x.GroupProperty;
    private static readonly Func<FactType2, object> ThrowGrouping = x => throw new InvalidOperationException("Grouping failed");
    private TestRule _testRule;

    public class FactType1
    {
        public string TestProperty { get; set; }
    }

    public class FactType2
    {
        public string JoinProperty { get; set; }
        public string GroupProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public Func<FactType2, object> Grouping = SuccessfulGrouping;

        public override void Define()
        {
            FactType1 fact1 = null;
            IEnumerable<FactType2> fact2Group = null;

            When()
                .Match(() => fact1)
                .Query(() => fact2Group, q => q
                    .Match<FactType2>(f => f.JoinProperty == fact1.TestProperty)
                    .GroupBy(f => Grouping(f))
                    .Select(x => x)
                );
            Then()
                .Do(ctx => NoOp());
        }

        private static void NoOp() { }
    }
}