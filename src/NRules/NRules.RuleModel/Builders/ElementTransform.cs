using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    public static class ElementTransform
    {
        public static RuleElement Normalize(ForAllElement element)
        {
            var sourcePattern = element.ChildElements.OfType<PatternElement>().Single();

            var notElements = new List<NotElement>();
            foreach (var condition in sourcePattern.Conditions)
            {
                var expression = condition.Expression;
                var negatedExpression = Expression.Lambda(Expression.Not(expression.Body), expression.Parameters);
                var negatedCondition = new ConditionElement(condition.Declarations, negatedExpression);
                var pattern = new PatternElement(sourcePattern.Declaration, new[] {negatedCondition});
                var not = new NotElement(new[] {pattern});
                notElements.Add(not);
            }

            var group = new AndElement(notElements);
            return group;
        }
    }
}