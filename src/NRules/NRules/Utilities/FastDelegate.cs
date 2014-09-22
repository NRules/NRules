using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Utilities
{
    internal static class FastDelegate
    {
        public static TDelegate Create<TDelegate>(LambdaExpression expression)
        {
            var optimizer = new ExpressionOptimizer<TDelegate>();
            var optimizedExpression = optimizer.CompactParameters(expression);
            var fastDelegate = optimizedExpression.Compile();
            return fastDelegate;
        }

        private class ExpressionOptimizer<TDelegate> : ExpressionVisitor
        {
            private ParameterExpression _arrayParameter;
            private Dictionary<ParameterExpression, int> _indexMap; 

            public Expression<TDelegate> CompactParameters(LambdaExpression expression)
            {
                _arrayParameter = Expression.Parameter(typeof(object[]));
                _indexMap = expression.Parameters.ToIndexMap();

                var body = Visit(expression.Body);
                var optimizedLambda = Expression.Lambda<TDelegate>(body, _arrayParameter);
                return optimizedLambda;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                int index = _indexMap[node];
                var arrayLookup = Expression.ArrayIndex(_arrayParameter, Expression.Constant(index));
                var parameterValue = Expression.Convert(arrayLookup, node.Type);
                return parameterValue;
            }
        }
    }
}
