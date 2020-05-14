using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.Rete;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Utilities
{
    internal static class ExpressionOptimizer
    {
        private static readonly MethodInfo GetEnumeratorMethod;
        private static readonly MethodInfo MoveNextMethod;
        private static readonly PropertyInfo CurrentProperty;
        private static readonly PropertyInfo FactValueProperty;

        static ExpressionOptimizer()
        {
            GetEnumeratorMethod = typeof(Tuple).GetTypeInfo()
                .GetDeclaredMethod(nameof(Tuple.GetEnumerator));
            MoveNextMethod = typeof(Tuple.Enumerator).GetTypeInfo()
                .GetDeclaredMethod(nameof(Tuple.Enumerator.MoveNext));
            CurrentProperty = typeof(Tuple.Enumerator).GetTypeInfo()
                .GetDeclaredProperty(nameof(Tuple.Enumerator.Current));
            FactValueProperty = typeof(Fact).GetTypeInfo()
                .GetDeclaredProperty(nameof(Fact.Object));
        }

        public static Expression<TDelegate> Optimize<TDelegate>(LambdaExpression expression)
        {
            return Optimize<TDelegate>(expression, null, tupleInput: false, factInput: true);
        }

        public static Expression<TDelegate> Optimize<TDelegate>(LambdaExpression expression,
            IndexMap indexMap, bool tupleInput, bool factInput)
        {
            return Optimize<TDelegate>(expression, 0, indexMap, tupleInput, factInput);
        }

        public static Expression<TDelegate> Optimize<TDelegate>(LambdaExpression expression,
            int startIndex, IndexMap indexMap, bool tupleInput, bool factInput)
        {
            var parts = new ExpressionParts(expression, startIndex);

            if (tupleInput)
                UnwrapTuple(parts, indexMap, factInput);

            if (factInput && indexMap != null)
                UnwrapFact(parts, indexMap);
            else if (factInput)
                UnwrapFact(parts);

            var invocation = EnsureReturnType(expression.Body, typeof(TDelegate));
            parts.Body.Add(invocation);

            var optimizedLambda = Expression.Lambda<TDelegate>(
                Expression.Block(parts.BodyParameters, parts.Body),
                parts.InputParameters);
            return optimizedLambda;
        }

        private static void UnwrapFact(ExpressionParts parts, IndexMap indexMap)
        {
            var factParameter = Expression.Parameter(typeof(Fact), "fact");
            parts.InputParameters.Add(factParameter);

            var parameterIndex = indexMap[indexMap.Length - 1];
            if (parameterIndex >= 0)
            {
                var bodyParameter = parts.BodyParameters[parameterIndex];

                parts.Body.Add(
                    AssignFactValue(factParameter, bodyParameter));
            }
        }

        private static void UnwrapFact(ExpressionParts parts)
        {
            var factParameter = Expression.Parameter(typeof(Fact), "fact");
            parts.InputParameters.Add(factParameter);

            parts.Body.Add(
                AssignFactValue(factParameter, parts.BodyParameters[0]));
        }

        private static void UnwrapTuple(ExpressionParts parts, IndexMap indexMap, bool factInput)
        {
            var tupleParameter = Expression.Parameter(typeof(Tuple), "tuple");
            parts.InputParameters.Add(tupleParameter);

            var enumerator = Expression.Variable(typeof(Tuple.Enumerator), "<enumerator>");
            parts.BodyParameters.Add(enumerator);

            parts.Body.Add(
                Expression.Assign(enumerator, Expression.Call(tupleParameter, GetEnumeratorMethod)));

            int tupleSize = indexMap.Length - 1;
            if (factInput) tupleSize--;
            for (int i = tupleSize; i >= 0; i--)
            {
                parts.Body.Add(
                    Expression.Call(enumerator, MoveNextMethod));

                var parameterIndex = indexMap[i];
                if (parameterIndex >= 0)
                {
                    var bodyParameter = parts.BodyParameters[parameterIndex];

                    var currentFact = Expression.Property(enumerator, CurrentProperty);
                    parts.Body.Add(
                        AssignFactValue(currentFact, bodyParameter));
                }
            }
        }

        private static BinaryExpression AssignFactValue(Expression fact, ParameterExpression parameter)
        {
            var factValue = Expression.Property(fact, FactValueProperty);
            return Expression.Assign(parameter, Expression.Convert(factValue, parameter.Type));
        }

        private static Expression EnsureReturnType(Expression expression, Type delegateType)
        {
            var returnType = delegateType.GetTypeInfo().GetDeclaredMethod(nameof(Action.Invoke)).ReturnType;
            if (returnType == typeof(void)) return expression;
            if (expression.Type == returnType) return expression;
            var convertedExpression = Expression.Convert(expression, returnType);
            return convertedExpression;
        }

        private readonly ref struct ExpressionParts
        {
            public ExpressionParts(LambdaExpression expression, int startIndex)
            {
                InputParameters = expression.Parameters.Take(startIndex).ToList();
                BodyParameters = expression.Parameters.Skip(startIndex).ToList();
                Body = new List<Expression>(2 + 2 * BodyParameters.Count);
            }

            public readonly List<ParameterExpression> InputParameters;
            public readonly List<ParameterExpression> BodyParameters;
            public readonly List<Expression> Body;
        }
    }
}