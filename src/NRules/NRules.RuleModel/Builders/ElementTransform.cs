using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    public static class ElementTransform
    {
        public static RuleElement Normalize(ForAllElement element)
        {
            var declarations = element.Source.Conditions.SelectMany(x => x.Declarations);
            var symbolTable = new SymbolTable(declarations);
            var groupBuilder = new GroupBuilder(symbolTable, GroupType.And);

            foreach (var condition in element.Source.Conditions)
            {
                var expression = condition.Expression;
                var negatedExpression = Expression.Lambda(Expression.Not(expression.Body), expression.Parameters);

                groupBuilder.Quantifier(QuantifierType.Not)
                    .SourcePattern(element.Source.ValueType)
                    .Condition(negatedExpression);
            }

            IBuilder<GroupElement> builder = groupBuilder;
            return builder.Build();
        }
    }
}