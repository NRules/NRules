using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NRules.Utilities
{
    internal abstract class FastDelegate
    {
        public static FastDelegate<TDelegate> Create<TDelegate>(LambdaExpression expression) where TDelegate : class 
        {
            if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
            {
                throw new InvalidOperationException(
                    string.Format("Type {0} is not a delegate", typeof(TDelegate).FullName));
            }

            var optimizer = new ExpressionOptimizer<TDelegate>();
            Expression<TDelegate> optimizedExpression = optimizer.CompactParameters(expression);
            TDelegate @delegate = optimizedExpression.Compile();
            var fastDelegate = new FastDelegate<TDelegate>(@delegate, expression.Parameters.Count);
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
                int index = _indexMap.IndexOrDefault(node);
                if (index >= 0)
                {
                    BinaryExpression arrayLookup = Expression.ArrayIndex(_arrayParameter, Expression.Constant(index));
                    UnaryExpression parameterValue = Expression.Convert(arrayLookup, node.Type);
                    return parameterValue;
                }
                return node;
            }
        }
    }

    internal class FastDelegate<TDelegate> : FastDelegate where TDelegate : class
    {
        private readonly TDelegate _delegate;
        private readonly int _parameterCount;

        public TDelegate Delegate
        {
            get { return _delegate; }
        }

        public int ParameterCount
        {
            get { return _parameterCount; }
        }

        internal FastDelegate(TDelegate @delegate, int parameterCount)
        {
            _delegate = @delegate;
            _parameterCount = parameterCount;
        }
    }
}