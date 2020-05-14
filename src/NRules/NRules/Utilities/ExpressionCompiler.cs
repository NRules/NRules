using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NRules.AgendaFilters;
using NRules.Aggregators;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Utilities
{
    internal static class ExpressionCompiler
    {
        public static ILhsExpression<TResult> CompileLhsExpression<TResult>(ExpressionElement element, List<Declaration> declarations)
        {
            if (element.Imports.Count() == 1 &&
                Equals(element.Imports.Single(), declarations.Last()))
            {
                return CompileLhsFactExpression<TResult>(element);
            }
            return CompileLhsTupleFactExpression<TResult>(element, declarations);
        }

        public static ILhsFactExpression<TResult> CompileLhsFactExpression<TResult>(ExpressionElement element)
        {
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Fact, TResult>>(
                element.Expression, null, tupleInput: false, factInput: true);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var expression = new LhsFactExpression<TResult>(element.Expression, fastDelegate);
            return expression;
        }

        public static ILhsTupleExpression<TResult> CompileLhsTupleExpression<TResult>(ExpressionElement element, IEnumerable<Declaration> declarations)
        {
            var factMap = IndexMap.CreateMap(element.Imports, declarations);
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Tuple, TResult>>(
                element.Expression, factMap, tupleInput: true, factInput: false);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var expression = new LhsTupleExpression<TResult>(element.Expression, fastDelegate);
            return expression;
        }

        public static ILhsExpression<TResult> CompileLhsTupleFactExpression<TResult>(ExpressionElement element, IEnumerable<Declaration> declarations)
        {
            var factMap = IndexMap.CreateMap(element.Imports, declarations);
            var optimizedExpression = ExpressionOptimizer.Optimize<Func<Tuple, Fact, TResult>>(
                element.Expression, factMap, tupleInput: true, factInput: true);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var expression = new LhsExpression<TResult>(element.Expression, fastDelegate);
            return expression;
        }

        public static IActivationExpression<TResult> CompileActivationExpression<TResult>(ExpressionElement element, IEnumerable<Declaration> declarations)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Func<object[], TResult>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 0);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count);
            var factMap = IndexMap.CreateMap(element.Imports, declarations);
            var expression = new ActivationExpression<TResult>(element.Expression, fastDelegate, factMap);
            return expression;
        }

        public static IRuleAction CompileAction(ActionElement element, IEnumerable<Declaration> declarations, IEnumerable<Declaration> dependencies)
        {
            var optimizer = new ExpressionMultiParameterOptimizer<Action<IContext, object[]>>();
            var optimizedExpression = optimizer.CompactParameters(element.Expression, 1);
            var @delegate = optimizedExpression.Compile();
            var fastDelegate = Create(@delegate, element.Expression.Parameters.Count - 1);
            var factMap = IndexMap.CreateMap(element.Imports, declarations);
            var dependencyIndexMap = IndexMap.CreateMap(element.Imports, dependencies);
            var action = new RuleAction(element.Expression, fastDelegate, factMap, dependencyIndexMap, element.ActionTrigger);
            return action;
        }

        public static IAggregateExpression CompileAggregateExpression(NamedExpressionElement element, List<Declaration> declarations)
        {
            var compiledExpression = CompileLhsExpression<object>(element, declarations);
            var expression = new AggregateExpression(element.Name, compiledExpression);
            return expression;
        }

        private static FastDelegate<TDelegate> Create<TDelegate>(TDelegate @delegate, int parameterCount) where TDelegate : class
        {
            return new FastDelegate<TDelegate>(@delegate, parameterCount);
        }

        private static Expression EnsureReturnType(Expression expression, Type delegateType)
        {
            var returnType = delegateType.GetTypeInfo().GetDeclaredMethod(nameof(Action.Invoke)).ReturnType;
            if (returnType == typeof(void)) return expression;
            if (expression.Type == returnType) return expression;
            var convertedExpression = Expression.Convert(expression, returnType);
            return convertedExpression;
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