using System.Linq.Expressions;

namespace NRules.Rule
{
    public class RuleAction
    {
        public LambdaExpression Expression { get; set; }

        internal RuleAction(LambdaExpression expression)
        {
            Expression = expression;
        }
    }
}