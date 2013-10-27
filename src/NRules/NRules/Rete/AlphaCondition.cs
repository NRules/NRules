using System.Linq.Expressions;

namespace NRules.Rete
{
    internal class AlphaCondition : Condition
    {
        public AlphaCondition(LambdaExpression expression) : base(expression)
        {
        }

        public bool IsSatisfiedBy(Fact fact)
        {
            return IsSatisfiedBy(fact.Object);
        }
    }
}