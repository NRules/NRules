using System;
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
    }

    internal class AggregateFactExpression : IAggregateExpression
    {
        private readonly LambdaExpression _expression;
        private readonly FastDelegate<Func<object, object>> _compiledExpression;

        public AggregateFactExpression(LambdaExpression expression, FastDelegate<Func<object, object>> compiledExpression)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
        }

        public object Invoke(AggregationContext context, ITuple tuple, IFact fact)
        {
            var factValue = fact.Value;
            try
            {
                var result = _compiledExpression.Delegate(factValue);
                context.EventAggregator.RaiseExpressionEvaluated(context.Session, _expression, null, factValue, result);
                return result;
            }
            catch (Exception e)
            {
                context.EventAggregator.RaiseExpressionEvaluated(context.Session, _expression, e, factValue, null);

                bool isHandled = false;
                context.EventAggregator.RaiseAggregateFailed(context.Session, e, _expression, tuple, fact, ref isHandled);
                throw new AggregateExpressionException(e, _expression, tuple, fact, isHandled);
            }
        }
    }

    internal class AggregateExpression : IAggregateExpression
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factMap;
        private readonly FastDelegate<Func<object[], object>> _compiledExpression;

        public AggregateExpression(LambdaExpression expression, FastDelegate<Func<object[], object>> compiledExpression, IndexMap factMap)
        {
            _expression = expression;
            _factMap = factMap;
            _compiledExpression = compiledExpression;
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

            try
            {
                var result = _compiledExpression.Delegate(args);
                context.EventAggregator.RaiseExpressionEvaluated(context.Session, _expression, null, args, result);
                return result;
            }
            catch (Exception e)
            {
                context.EventAggregator.RaiseExpressionEvaluated(context.Session, _expression, e, args, null);

                bool isHandled = false;
                context.EventAggregator.RaiseAggregateFailed(context.Session, e, _expression, tuple, fact, ref isHandled);
                throw new AggregateExpressionException(e, _expression, tuple, fact, isHandled);
            }
        }

        public override string ToString()
        {
            return _expression.ToString();
        }
    }
}
