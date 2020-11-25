using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.IntegrationTests.TestAssets;
using NRules.RuleModel;
using Xunit;

namespace NRules.IntegrationTests
{
    public class BindingEvaluationExceptionTest : BaseRuleTestFixture
    {
        [Fact]
        public void Insert_ErrorInBindingNoErrorHandler_Throws()
        {
            //Arrange
            GetRuleInstance<TestRule>().Binding = ThrowBinding;

            Expression expression = null;
            IList<IFact> facts = null;
            Session.Events.LhsExpressionFailedEvent += (sender, args) => expression = args.Expression;
            Session.Events.LhsExpressionFailedEvent += (sender, args) => facts = args.Facts.ToList();

            var fact = new FactType { TestProperty = "Valid value" };

            //Act - Assert
            var ex = Assert.Throws<RuleLhsExpressionEvaluationException>(() => Session.Insert(fact));
            Assert.NotNull(expression);
            Assert.Equal(1, facts.Count);
            Assert.Same(fact, facts.First().Value);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        [Fact]
        public void Fire_FailedAssert_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().Binding = ThrowBinding;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType { TestProperty = "Valid value" };

            Session.Insert(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FailedAssertThenAssertAnother_Fires()
        {
            //Arrange
            GetRuleInstance<TestRule>().Binding = ThrowBinding;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact1 = new FactType { TestProperty = "Valid value" };
            Session.Insert(fact1);

            GetRuleInstance<TestRule>().Binding = SuccessfulBinding;
            
            var fact2 = new FactType { TestProperty = "Valid value" };
            Session.Insert(fact2);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_FailedAssertThenUpdate_Fires()
        {
            //Arrange
            GetRuleInstance<TestRule>().Binding = ThrowBinding;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType {TestProperty = "Valid value"};

            Session.Insert(fact);
            GetRuleInstance<TestRule>().Binding = SuccessfulBinding;

            Session.Update(fact);

            //Act
            Session.Fire();

            //Assert
            AssertFiredOnce();
        }

        [Fact]
        public void Fire_FailedAssertThenUpdateThenRetract_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().Binding = ThrowBinding;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType {TestProperty = "Valid value"};

            Session.Insert(fact);
            GetRuleInstance<TestRule>().Binding = SuccessfulBinding;

            Session.Update(fact);
            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        [Fact]
        public void Fire_FailedAssertThenRetract_DoesNotFire()
        {
            //Arrange
            GetRuleInstance<TestRule>().Binding = ThrowBinding;

            Session.Events.LhsExpressionFailedEvent += (sender, args) => args.IsHandled = true;

            var fact = new FactType {TestProperty = "Valid value"};

            Session.Insert(fact);
            GetRuleInstance<TestRule>().Binding = SuccessfulBinding;

            Session.Retract(fact);

            //Act
            Session.Fire();

            //Assert
            AssertDidNotFire();
        }

        protected override void SetUpRules()
        {
            SetUpRule<TestRule>();
        }

        private static readonly Func<FactType, string> SuccessfulBinding = f => "value";
        private static readonly Func<FactType, string> ThrowBinding = f => throw new InvalidOperationException("Binding failed");

        public class FactType
        {
            public string TestProperty { get; set; }
        }

        public class TestRule : Rule
        {
            public Func<FactType, string> Binding = SuccessfulBinding;

            public override void Define()
            {
                FactType fact = null;
                string binding = null;

                When()
                    .Match(() => fact, f => f.TestProperty.StartsWith("Valid"))
                    .Let(() => binding, () => Binding(fact));
                Then()
                    .Do(ctx => NoOp());
            }

            private static void NoOp() { }
        }
    }
}