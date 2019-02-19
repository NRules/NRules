using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Aggregators
{
    /// <summary>
    /// Expression used by an aggregator, compiled to an executable form.
    /// </summary>
    public interface IAggregateExpression
    {
        /// <summary>
        /// Invokes the expression with the given inputs.
        /// </summary>
        /// <param name="context">Aggregation context.</param>
        /// <param name="tuple">Partial match up to the aggregate element.</param>
        /// <param name="fact">Fact being processed by the aggregate element.</param>
        /// <returns>Result of the expression.</returns>
        object Invoke(AggregationContext context, ITuple tuple, IFact fact);

        /// <summary>
        /// Name of the aggregate expression.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Extension methods used for working with collections of aggregate expressions.
    /// </summary>
    public static class IAggregateExpressionExtensions
    {
        /// <summary>
        /// Get an enumerable of matching aggregate expressions.
        /// </summary>
        /// <param name="expressions">The list of aggregate expressions to search through.</param>
        /// <param name="name">Name of the aggregate expressions to find.</param>
        /// <returns></returns>
        public static IEnumerable<IAggregateExpression> Find(this IEnumerable<IAggregateExpression> expressions, string name)
        {
            return expressions.Where(e => e.Name == name);
        }

        /// <summary>
        /// Get a single matching aggregate expression.
        /// </summary>
        /// <param name="expressions">The list of aggregate expressions to search through.</param>
        /// <param name="name">Name of the aggregate expression to find.</param>
        /// <returns></returns>
        public static IAggregateExpression FindSingle(this IEnumerable<IAggregateExpression> expressions, string name)
        {
            return expressions.Find(name).Single();
        }
    }

    internal class AggregateFactExpression : IAggregateExpression
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object, object>> _compiledExpression;

        public AggregateFactExpression(string name, LambdaExpression expression, FastDelegate<Func<object, object>> compiledExpression)
        {
            Name = name;
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public string Name { get; }

        public object Invoke(AggregationContext context, ITuple tuple, IFact fact)
        {
            var factValue = fact.Value;
            Exception exception = null;
            object result = null;
            try
            {
                result = _compiledExpression.Delegate(factValue);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, factValue, tuple, fact, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, factValue, result, tuple, fact);
            }
        }
    }

    internal class AggregateExpression : IAggregateExpression
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factMap;
        private readonly FastDelegate<Func<object[], object>> _compiledExpression;

        public AggregateExpression(string name, LambdaExpression expression, FastDelegate<Func<object[], object>> compiledExpression, IndexMap factMap)
        {
            _expression = expression;
            _factMap = factMap;
            _compiledExpression = compiledExpression;
            Name = name;
        }

        public object Invoke(AggregationContext context, ITuple tuple, IFact fact)
        {
            var args = new object[_compiledExpression.ArrayArgumentCount];
            int index = tuple.Count - 1;
            foreach (var tupleFact in tuple.Facts)
            {
                IndexMap.SetElementAt(args, _factMap[index], tupleFact.Value);
                index--;
            }
            IndexMap.SetElementAt(args, _factMap[tuple.Count], fact.Value);

            Exception exception = null;
            object result = null;
            try
            {
                result = _compiledExpression.Delegate(args);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, args, tuple, fact, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, args, result, tuple, fact);
            }
        }

        public string Name { get; }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}
