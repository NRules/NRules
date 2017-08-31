using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Aggregators;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Utilities
{
    internal abstract class ExpressionCompiler
    {
        public static IAlphaCondition CompileAlphaCondition(ConditionElement element)
        {
            var optimizer = new ExpressionSingleParameterOptimizer<Func<object, bool>>();
            var optimizedExpression = optimizer.ConvertParameter(element.Expression);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var condition = new AlphaCondition(element.Expression, fastDelegate);
            return condition;
        }

        public static IBetaCondition CompileBetaCondition(ConditionElement element, IEnumerable<Declaration> declarations)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Func<object[], bool>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 0);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var factIndexMap = IndexMap.CreateMap(element.References, declarations);
            var condition = new BetaCondition(element.Expression, fastDelegate, factIndexMap);
            return condition;
        }

        public static IRuleAction CompileAction(ActionElement element, IEnumerable<Declaration> declarations, IEnumerable<Declaration> dependencies)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Action<IContext, object[]>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 1);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count - 1);
            var factIndexMap = IndexMap.CreateMap(element.References, declarations);
            var dependencyIndexMap = IndexMap.CreateMap(element.References, dependencies);
            var action = new RuleAction(element.Expression, fastDelegate, factIndexMap, dependencyIndexMap);
            return action;
        }

        public static IAggregateExpression CompileAggregateExpression(NamedExpressionElement element, IEnumerable<Declaration> declarations)
        {
            var declarationsList = declarations.ToList();
            if (element.References.Count() == 1 &&
                Equals(element.References.Single(), declarationsList.Last()))
            {
                var optimizer = new ExpressionSingleParameterOptimizer<Func<object, object>>();
                var optimizedExpression = optimizer.ConvertParameter(element.Expression);
                var @delegate = optimizedExpression.Compile();
                var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
                var expression = new AggregateFactExpression(element.Expression, fastDelegate);
                return expression;
            }
            else
            {
                var optimizer = new ExpressionMultiParameterOptimizer<Func<object[], object>>();
                var optimizedExpression = optimizer.CompactParameters(element.Expression, 0);
                var @delegate = optimizedExpression.Compile();
                var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
                var factIndexMap = IndexMap.CreateMap(element.References, declarationsList);
                var expression = new AggregateExpression(element.Expression, fastDelegate, factIndexMap);
                return expression;
            }
        }

        public static IBindingExpression CompileBindingExpression(BindingElement element, IEnumerable<Declaration> declarations)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Func<object[], object>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 0);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var factIndexMap = IndexMap.CreateMap(element.References, declarations);
            var expression = new BindingExpression(element.Expression, fastDelegate, factIndexMap);
            return expression;
        }

        public static IActivationCondition CompileFilterCondition(FilterElement element, IEnumerable<Declaration> declarations)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Func<object[], bool>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 0);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var factIndexMap = IndexMap.CreateMap(element.References, declarations);
            var expression = new ActivationCondition(element.Expression, fastDelegate, factIndexMap);
            return expression;
        }

        public static IActivationExpression CompileFilterExpression(FilterElement element, IEnumerable<Declaration> declarations)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Func<object[], object>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 0);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var factIndexMap = IndexMap.CreateMap(element.References, declarations);
            var expression = new ActivationExpression(element.Expression, fastDelegate, factIndexMap);
            return expression;
        }
        
        private static FastDelegate<TDelegate> Create<TDelegate>(TDelegate @delegate, int parameterCount) where TDelegate : class
        {
            return new FastDelegate<TDelegate>(@delegate, parameterCount);
        }

        private static Expression EnsureReturnType(Expression expression, Type delegateType)
        {
            if (expression.Type == typeof(void)) return expression;
            if (delegateType.GenericTypeArguments.Length == 0) return expression;

            var resultType = delegateType.GenericTypeArguments.Last();
            if (expression.Type == resultType) return expression;
            var convertedExpression = Expression.Convert(expression, resultType);
            return convertedExpression;
        }

        private class ExpressionSingleParameterOptimizer<TDelegate> : ExpressionVisitor
        {
            private ParameterExpression _objectParameter;
            private ParameterExpression _typedParameter;

            /// <summary>
            /// Transforms expression from single typed parameter to single object parameter,
            /// which allows execution w/o reflection.
            /// </summary>
            /// <param name="expression">Expression to transform.</param>
            /// <returns>Transformed expression.</returns>
            public Expression<TDelegate> ConvertParameter(LambdaExpression expression)
            {
                _objectParameter = Expression.Parameter(typeof (object));
                _typedParameter = expression.Parameters.Single();

                var body = Visit(expression.Body);
                var convertedBody = EnsureReturnType(body, typeof(TDelegate));

                Expression<TDelegate> optimizedLambda = Expression.Lambda<TDelegate>(convertedBody, _objectParameter);
                return optimizedLambda;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _typedParameter)
                {
                    UnaryExpression parameterValue = Expression.Convert(_objectParameter, node.Type);
                    return parameterValue;
                }
                return node;
            }
        }

        private class ExpressionMultiParameterOptimizer<TDelegate> : ExpressionVisitor
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
                _arrayParameter = Expression.Parameter(typeof(object[]));
                _indexMap = expression.Parameters.Skip(startIndex).ToIndexMap();

                var parameters = expression.Parameters.Take(startIndex).ToList();
                parameters.Add(_arrayParameter);

                var body = Visit(expression.Body);
                var convertedBody = EnsureReturnType(body, typeof(TDelegate));

                Expression<TDelegate> optimizedLambda = Expression.Lambda<TDelegate>(convertedBody, parameters);
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
}