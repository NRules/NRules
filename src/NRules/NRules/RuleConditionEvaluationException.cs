using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Events;
using NRules.Rete;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule condition.
    /// </summary>
    public class RuleConditionEvaluationException : RuleExpressionEvaluationException
    {
        private readonly Rete.Tuple _tuple;
        private readonly Fact _fact;

        internal RuleConditionEvaluationException(string message, LambdaExpression conditionExpression, Fact fact, Exception innerException)
            : this(message, conditionExpression, null, fact, innerException)
        {
        }

        internal RuleConditionEvaluationException(string message, LambdaExpression conditionExpression, Rete.Tuple tuple, Fact fact, Exception innerException)
            : base(message, conditionExpression, innerException)
        {
            _tuple = tuple;
            _fact = fact;
        }

        /// <summary>
        /// Condition that caused exception.
        /// </summary>
        public string Condition
        {
            get { return Expression.ToString(); }
        }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<FactInfo> Facts
        {
            get
            {
                var wrappedFact = new []{new FactInfo(_fact) };
                return _tuple == null
                    ? wrappedFact
                    : _tuple.Facts.Reverse().Select(x => new FactInfo(x)).Concat(wrappedFact).ToArray();
            }
        }
    }
}