using System.Linq;
using System.Linq.Expressions;

namespace NRules.RuleModel.Builders
{
    public static class ElementTransform
    {
        public static RuleElement Normalize(ForAllElement element)
        {
            //forall -> not(base and not(patterns))
            var declarations = Enumerable.Repeat(element.BasePattern, 1).Union(element.Patterns)
                .SelectMany(x => x.Conditions).SelectMany(x => x.Declarations);
            var symbolTable = new SymbolTable(declarations);

            var notBuilder = new NotBuilder(symbolTable);
            var groupBuilder = notBuilder.Group();

            Declaration declaration = element.BasePattern.Declaration;
            var basePatternBuilder = groupBuilder.Pattern(declaration);
            foreach (var condition in element.BasePattern.Conditions)
            {
                basePatternBuilder.Condition(condition.Expression);
            }

            var baseParameter = basePatternBuilder.Declaration.ToParameterExpression();

            foreach (var pattern in element.Patterns)
            {
                var patternBuilder = groupBuilder
                    .Not()
                    .Pattern(pattern.Declaration);
                
                var parameter = patternBuilder.Declaration.ToParameterExpression();
                //Join is required to correlate negated patterns with the base pattern
                patternBuilder.Condition(
                    Expression.Lambda(
                        Expression.ReferenceEqual(baseParameter, parameter), 
                        baseParameter, parameter));
                
                foreach (var condition in pattern.Conditions)
                {
                    patternBuilder.Condition(condition.Expression);
                }
            }

            IBuilder<NotElement> builder = notBuilder;
            return builder.Build();
        }
    }
}