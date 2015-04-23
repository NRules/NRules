using System;
using System.Linq.Expressions;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using NRules.RuleModel.Builders;

namespace NRules.Fluent.Expressions
{
    internal class RightHandSideExpression : IRightHandSideExpression
    {
        private readonly RuleBuilder _builder;

        public RightHandSideExpression(RuleBuilder builder)
        {
            _builder = builder;
        }

        public IRightHandSideExpression Do(Expression<Action<IContext>> action)
        {
            var rightHandSide = _builder.RightHandSide();
            rightHandSide.DslAction(rightHandSide.Declarations, action);
            return this;
        }
    }
}