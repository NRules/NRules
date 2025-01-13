using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class BindingEvaluationExceptionTest : BaseRulesTestFixture
{
    [Fact]
    public void Insert_ErrorInBindingNoErrorHandler_Throws()
    {
        //Arrange
        _testRule.Binding = ThrowBinding;

        Expression expression = null!;
        IList<IFact> facts = null!;
        Session.Events.LhsExpressionFailedEvent += (sender, args) => expression = args.Expression;
        Session.Events.LhsExpressionFailedEvent += (sender, args) => facts = args.Facts.ToList();

        var fact = new FactType { TestProperty = "Valid value" };

        //Act - Assert
        var ex = Assert.Throws<RuleLhsExpressionEvaluationException>(() => Session.Insert(fact));
        Assert.NotNull(expression);
        Assert.Single(facts);
        Assert.Same(fact, facts.First().Value);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void Fire_FailedAssert_DoesNotFire()
    {
        //Arrange
        _testRule.Binding = ThrowBinding;

        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact = new FactType { TestProperty = "Valid value" };

        Session.Insert(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FailedAssertThenAssertAnother_Fires()
    {
        //Arrange
        _testRule.Binding = ThrowBinding;

        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact1 = new FactType { TestProperty = "Valid value" };
        Session.Insert(fact1);

        _testRule.Binding = SuccessfulBinding;

        var fact2 = new FactType { TestProperty = "Valid value" };
        Session.Insert(fact2);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_FailedAssertThenUpdate_Fires()
    {
        //Arrange
        _testRule.Binding = ThrowBinding;

        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact = new FactType { TestProperty = "Valid value" };

        Session.Insert(fact);
        _testRule.Binding = SuccessfulBinding;

        Session.Update(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
    }

    [Fact]
    public void Fire_FailedAssertThenUpdateThenRetract_DoesNotFire()
    {
        //Arrange
        _testRule.Binding = ThrowBinding;

        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact = new FactType { TestProperty = "Valid value" };

        Session.Insert(fact);
        _testRule.Binding = SuccessfulBinding;

        Session.Update(fact);
        Session.Retract(fact);

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired(Times.Never));
    }

    [Fact]
    public void Fire_FailedAssertThenRetract_DoesNotFire()
    {
        //Arrange
        _testRule.Binding = ThrowBinding;

        Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

        var fact = new FactType { TestProperty = "Valid value" };

        Session.Insert(fact);
        _testRule.Binding = SuccessfulBinding;

        Session.Retract(fact);

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

    private static readonly Func<FactType, string> SuccessfulBinding = f => "value";
    private static readonly Func<FactType, string> ThrowBinding = f => throw new InvalidOperationException("Binding failed");
    private TestRule _testRule = null!;

    public class FactType
    {
        [NotNull]
        public string? TestProperty { get; set; }
    }

    public class TestRule : Rule
    {
        public Func<FactType, string> Binding = SuccessfulBinding;

        public override void Define()
        {
            FactType fact = null!;
            string binding = null!;

            When()
                .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                .Let(() => binding, () => Binding(fact));
            Then()
                .Do(ctx => NoOp());
        }

        private static void NoOp() { }
    }
}