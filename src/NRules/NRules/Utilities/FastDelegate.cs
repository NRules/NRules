using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Utilities
{
    internal static class FastDelegate
    {
        public static TDelegate Create<TDelegate>(LambdaExpression expression)
        {
            var optimizer = new ExpressionOptimizer<TDelegate>();
            Expression<TDelegate> optimizedExpression = optimizer.CompactParameters(expression);
            TDelegate fastDelegate = optimizedExpression.Compile();
            return fastDelegate;
        }

        private class ExpressionOptimizer<TDelegate> : ExpressionVisitor
        {
            private ParameterExpression _arrayParameter;
            private Dictionary<ParameterExpression, int> _indexMap;

            /// <summary>
            /// Transforms expression from multi-parameter to single array parameter,
            /// which allows execution w/o reflection.
            /// </summary>
            /// <param name="expression">Expression to transform.</param>
            /// <returns>Transformed expression.</returns>
            public Expression<TDelegate> CompactParameters(LambdaExpression expression)
            {
                _arrayParameter = Expression.Parameter(typeof (object[]));
                _indexMap = expression.Parameters.ToIndexMap();

                Expression body = Visit(expression.Body);
                Expression<TDelegate> optimizedLambda = Expression.Lambda<TDelegate>(body, _arrayParameter);
                return optimizedLambda;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                int index = _indexMap[node];
                BinaryExpression arrayLookup = Expression.ArrayIndex(_arrayParameter, Expression.Constant(index));
                UnaryExpression parameterValue = Expression.Convert(arrayLookup, node.Type);
                return parameterValue;
            }
        }
    }
}