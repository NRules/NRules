using System;
using System.Linq.Expressions;
using NRules.Dsl;

namespace NRules.Rule
{
    public class ActionElement
    {
        public LambdaExpression Expression { get; set; }

        internal ActionElement(Expression<Action<IActionContext>> expression)
        {
            Expression = expression;
        }
    }
}