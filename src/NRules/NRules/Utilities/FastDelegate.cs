using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;

namespace NRules.Utilities
{
    internal abstract class FastDelegate
    {
        public static FastDelegate<Func<object[], bool>> Condition(LambdaExpression expression)
        {
            var optimizer = new ExpressionOptimizer<Func<object[], bool>>();
            var optimizedExpression = optimizer.CompactParameters(expression, 0);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, expression.Parameters.Count);
            return fastDelegate;
        }

        public static FastDelegate<Action<IContext, object[]>> Action(LambdaExpression expression)
        {
            var optimizer = new ExpressionOptimizer<Action<IContext, object[]>>();
            var optimizedExpression = optimizer.CompactParameters(expression, 1);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, expression.Parameters.Count - 1);
            return fastDelegate;
        }

        private static FastDelegate<TDelegate> Create<TDelegate>(TDelegate @delegate, int parameterCount) where TDelegate : class
        {
            return new FastDelegate<TDelegate>(@delegate, parameterCount);
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
            /// <param name="startIndex">Index of the first parameter to compact into an array.</param>
            /// <returns>Transformed expression.</returns>
            public Expression<TDelegate> CompactParameters(LambdaExpression expression, int startIndex)
            {
                _arrayParameter = Expression.Parameter(typeof (object[]));
                _indexMap = expression.Parameters.Skip(startIndex).ToIndexMap();

                var parameters = expression.Parameters.Take(startIndex).ToList();
                parameters.Add(_arrayParameter);

                Expression body = Visit(expression.Body);
                Expression<TDelegate> optimizedLambda = Expression.Lambda<TDelegate>(body, parameters);
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
        private readonly int _arrayArgumentCount;

        public TDelegate Delegate
        {
            get { return _delegate; }
        }

        public int ArrayArgumentCount
        {
            get { return _arrayArgumentCount; }
        }

        internal FastDelegate(TDelegate @delegate, int arrayArgumentCount)
        {
            _delegate = @delegate;
            _arrayArgumentCount = arrayArgumentCount;
        }
    }
}