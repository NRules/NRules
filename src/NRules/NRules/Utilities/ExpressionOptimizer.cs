using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.Extensibility;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Utilities
{
    internal static class ExpressionOptimizer
    {
        private static readonly MethodInfo GetEnumeratorMethod;
        private static readonly MethodInfo MoveNextMethod;
        private static readonly PropertyInfo CurrentProperty;
        private static readonly PropertyInfo FactValueProperty;
        private static readonly MethodInfo ResolveMethod;

        static ExpressionOptimizer()
        {
            GetEnumeratorMethod = typeof(Tuple)
                .GetMethod(nameof(Tuple.GetEnumerator));
            MoveNextMethod = typeof(Tuple.Enumerator)
                .GetMethod(nameof(Tuple.Enumerator.MoveNext));
            CurrentProperty = typeof(Tuple.Enumerator)
                .GetProperty(nameof(Tuple.Enumerator.Current));
            FactValueProperty = typeof(Fact)
                .GetProperty(nameof(Fact.Object));
            ResolveMethod = typeof(IDependencyResolver)
                .GetMethod(nameof(IDependencyResolver.Resolve));
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
                UnwrapTuple(parts, indexMap, factInput ? indexMap.Length - 1 : indexMap.Length);

            if (factInput)
                UnwrapFact(parts, indexMap);

            var invocation = EnsureReturnType(expression.Body, typeof(TDelegate));
            parts.Body.Add(invocation);

            var optimizedLambda = Expression.Lambda<TDelegate>(
                Expression.Block(parts.BodyParameters, parts.Body),
                parts.InputParameters);
            return optimizedLambda;
        }

        public static Expression<TDelegate> Optimize<TDelegate>(LambdaExpression expression,
            IndexMap factIndexMap, List<DependencyElement> dependencies, IndexMap dependencyIndexMap)
        {
            var parts = new ExpressionParts(expression, 1);
            
            UnwrapTuple(parts, factIndexMap, factIndexMap.Length);
            ResolveDependencies(parts, dependencies, dependencyIndexMap);

            var invocation = EnsureReturnType(expression.Body, typeof(TDelegate));
            parts.Body.Add(invocation);

            var optimizedLambda = Expression.Lambda<TDelegate>(
                Expression.Block(parts.BodyParameters, parts.Body),
                parts.InputParameters);
            return optimizedLambda;
        }

        private static void UnwrapFact(ExpressionParts parts, IndexMap indexMap)
        {
            var factParameter = Expression.Parameter(typeof(Fact), "<fact>");
            parts.InputParameters.Add(factParameter);

            var parameterIndex = indexMap[indexMap.Length - 1];
            if (parameterIndex >= 0)
            {
                var bodyParameter = parts.BodyParameters[parameterIndex];

                parts.Body.Add(
                    AssignFactValue(factParameter, bodyParameter));
            }
        }

        private static void UnwrapTuple(ExpressionParts parts, IndexMap indexMap, int tupleSize)
        {
            var tupleParameter = Expression.Parameter(typeof(Tuple), "<tuple>");
            parts.InputParameters.Add(tupleParameter);

            if (tupleSize <= 0) return;

            var enumerator = Expression.Variable(typeof(Tuple.Enumerator), "<enumerator>");
            parts.BodyParameters.Add(enumerator);

            parts.Body.Add(
                Expression.Assign(enumerator, Expression.Call(tupleParameter, GetEnumeratorMethod)));

            var intermediateParts = new List<Expression>(tupleSize);
            for (int i = tupleSize - 1; i >= 0; i--)
            {
                intermediateParts.Add(
                    Expression.Call(enumerator, MoveNextMethod));

                var parameterIndex = indexMap[i];
                if (parameterIndex >= 0)
                {
                    parts.Body.AddRange(intermediateParts);
                    intermediateParts.Clear();

                    var bodyParameter = parts.BodyParameters[parameterIndex];

                    var currentFact = Expression.Property(enumerator, CurrentProperty);
                    parts.Body.Add(
                        AssignFactValue(currentFact, bodyParameter));
                }
            }
        }

        private static void ResolveDependencies(ExpressionParts parts, 
            List<DependencyElement> dependencies, IndexMap indexMap)
        {
            var resolverParameter = Expression.Parameter(typeof(IDependencyResolver), "<resolver>");
            parts.InputParameters.Add(resolverParameter);
            var resolutionContextParameter = Expression.Parameter(typeof(IResolutionContext), "<resolutionContext>");
            parts.InputParameters.Add(resolutionContextParameter);

            for (int i = 0; i <= indexMap.Length; i++)
            {
                var parameterIndex = indexMap[i];
                if (parameterIndex >= 0)
                {
                    var parameter = parts.BodyParameters[parameterIndex];
                    var parameterType = dependencies[i].ServiceType;
                    var resolveDependency = Expression.Call(
                        resolverParameter, ResolveMethod, resolutionContextParameter, Expression.Constant(parameterType));
                    parts.Body.Add(
                        Expression.Assign(parameter, Expression.Convert(resolveDependency, parameterType)));
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
            var invokeMethod = delegateType.GetMethod(nameof(Action.Invoke));
            if (invokeMethod == null) return expression;

            var returnType = invokeMethod.ReturnType;
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