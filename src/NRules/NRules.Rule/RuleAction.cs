using System;
using System.Linq.Expressions;
using NRules.Dsl;

namespace NRules.Rule
{
    public interface IRuleAction
    {
        void Invoke(IActionContext context);
    }

    internal class RuleAction : IRuleAction
    {
        private readonly Action<IActionContext> _compiledExpression;

        public LambdaExpression Expression { get; set; }

        internal RuleAction(Expression<Action<IActionContext>> expression)
        {
            Expression = expression;
            _compiledExpression = expression.Compile();
        }

        public void Invoke(IActionContext context)
        {
            _compiledExpression.Invoke(context);
        }
    }
}