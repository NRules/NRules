﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NRules.Extensibility;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using NRules.Testing;
using Xunit;

namespace NRules.IntegrationTests;

public class ActionInterceptorTest : BaseRulesTestFixture
{
    private TestRule _testRule = null!;

    [Fact]
    public void Fire_ConditionsMatchNoInterceptor_ExecutesAction()
    {
        //Arrange
        var fact1 = new FactType1();
        var fact2 = new FactType2();
        Session.Insert(fact1);
        Session.Insert(fact2);

        bool actionExecuted = false;
        _testRule.Action = () => { actionExecuted = true; };

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.True(actionExecuted);
    }

    [Fact]
    public void Fire_ConditionsMatchInterceptorInvokes_ExecutesAction()
    {
        //Arrange
        Session.ActionInterceptor = new ActionInterceptor(invoke: true);

        var fact1 = new FactType1();
        var fact2 = new FactType2();
        Session.Insert(fact1);
        Session.Insert(fact2);

        bool actionExecuted = false;
        _testRule.Action = () => { actionExecuted = true; };

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.True(actionExecuted);
    }

    [Fact]
    public void Fire_InterceptorInvokesActionThatThrows_ExceptionNotWrapped()
    {
        //Arrange
        Session.ActionInterceptor = new ActionInterceptor(invoke: true);

        var fact1 = new FactType1();
        var fact2 = new FactType2();
        Session.Insert(fact1);
        Session.Insert(fact2);

        _testRule.Action = () => { throw new InvalidOperationException("Test"); };

        //Act - Assert
        var ex = Assert.Throws<InvalidOperationException>(() => Session.Fire());
        Assert.Equal("Test", ex.Message);
    }

    [Fact]
    public void Fire_ConditionsMatchInterceptorDoesNotInvoke_DoesNotExecuteAction()
    {
        //Arrange
        Session.ActionInterceptor = new ActionInterceptor(invoke: false);

        var fact1 = new FactType1();
        var fact2 = new FactType2();
        Session.Insert(fact1);
        Session.Insert(fact2);

        bool actionExecuted = false;
        _testRule.Action = () => { actionExecuted = true; };

        //Act
        Session.Fire();

        //Assert
        Verify(x => x.Rule().Fired());
        Assert.False(actionExecuted);
    }

    protected override void SetUpRules(IRulesTestSetup setup)
    {
        _testRule = new TestRule();
        setup.Rule(_testRule);
    }

    private class ActionInterceptor : IActionInterceptor
    {
        private readonly bool _invoke;

        public ActionInterceptor(bool invoke)
        {
            _invoke = invoke;
        }

        public void Intercept(IContext context, IReadOnlyCollection<IActionInvocation> actions)
        {
            if (_invoke)
            {
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }
    }

    public class FactType1
    {
    }

    public class FactType2
    {
    }

    public class TestRule : Rule
    {
        public Action Action = () => { };

        public override void Define()
        {
            FactType1 fact = null!;
            IEnumerable<FactType2> collection = null!;

            When()
                .Match(() => fact)
                .Query(() => collection, x => x
                    .Match<FactType2>()
                    .Collect());
            Then()
                .Do(ctx => CallAction(ctx, fact, collection));
        }

        private void CallAction(IContext context, FactType1 fact1, IEnumerable<FactType2> collection2)
        {
            Action();
        }
    }
}
