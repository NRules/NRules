using System.Linq.Expressions;

namespace NRules.Rete
{
    internal interface IAlphaCondition
    {
        bool IsSatisfiedBy(Fact fact);
    }

    internal class AlphaCondition : Condition, IAlphaCondition
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