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
        /// <param name="tuple">Partial match up to the aggregate element.</param>
        /// <param name="fact">Fact being processed by the aggregate element.</param>
        /// <returns>Result of the expression.</returns>
        object Invoke(ITuple tuple, IFact fact);
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

        public object Invoke(ITuple tuple, IFact fact)
        {
            try
            {
                var factValue = fact.Value;
                var result = _compiledExpression.Delegate(factValue);
                return result;
            }
            catch (Exception e)
            {
                throw new RuleExpressionEvaluationException("Failed to evaluate expression", _expression.ToString(), e);
            }
        }
    }

    internal class AggregateExpression : IAggregateExpression
    {
        private readonly LambdaExpression _expression;
        private readonly IndexMap _factIndexMap;
        private readonly FastDelegate<Func<object[], object>> _compiledExpression;

        public AggregateExpression(LambdaExpression expression, FastDelegate<Func<object[], object>> compiledExpression, IndexMap factIndexMap)
        {
            _expression = expression;
            _factIndexMap = factIndexMap;
            _compiledExpression = compiledExpression;
        }

        public object Invoke(ITuple tuple, IFact fact)
        {
            var args = new object[_compiledExpression.ArrayArgumentCount];
            int index = tuple.Count - 1;
            foreach (var tupleFact in tuple.Facts)
            {
                IndexMap.SetElementAt(args, _factIndexMap[index], tupleFact.Value);
                index--;
            }
            IndexMap.SetElementAt(args, _factIndexMap[tuple.Count], fact.Value);

            try
            {
                var result = _compiledExpression.Delegate(args);
                return result;
            }
            catch (Exception e)
            {
                throw new RuleExpressionEvaluationException("Failed to evaluate expression", _expression.ToString(), e);
            }
        }
    }
}
