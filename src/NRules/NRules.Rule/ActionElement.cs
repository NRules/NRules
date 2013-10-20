using System.Linq.Expressions;

namespace NRules.Rule
{
    public class ActionElement
    {
        public LambdaExpression Expression { get; set; }

        internal ActionElement(LambdaExpression expression)
        {
            Expression = expression;
        }
    }
}