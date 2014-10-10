using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Events;
using Tuple = NRules.Rete.Tuple;

namespace NRules
{
    /// <summary>
    /// Represents errors that occur while evaluating rule action.
    /// </summary>
    public class RuleActionEvaluationException : RuleExpressionEvaluationException
    {
        private readonly Tuple _tuple;

        internal RuleActionEvaluationException(string message, LambdaExpression actionExpression, Rete.Tuple tuple, Exception innerException)
            : base(message, actionExpression, innerException)
        {
            _tuple = tuple;
        }

        /// <summary>
        /// Action that caused exception.
        /// </summary>
        public string Action
        {
            get { return Expression.ToString(); }
        }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<FactInfo> Facts { get { return _tuple.Facts.Reverse().Select(x => new FactInfo(x)).ToArray(); } }
    }
}