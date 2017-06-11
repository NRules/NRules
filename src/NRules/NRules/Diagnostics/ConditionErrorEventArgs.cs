using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during condition evaluation.
    /// </summary>
    public class ConditionErrorEventArgs : ErrorEventArgs
    {
        private readonly Tuple _tuple;
        private readonly Fact _fact;

        internal ConditionErrorEventArgs(Exception exception, Expression expression, Tuple tuple, Fact fact)
            : base(exception)
        {
            Condition = expression;
            _tuple = tuple;
            _fact = fact;
        }

        /// <summary>
        /// Condition that caused exception.
        /// </summary>
        public Expression Condition { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFact> Facts
        {
            get
            {
                var wrappedFact = new[] {_fact};
                return _tuple == null
                    ? wrappedFact
                    : _tuple.OrderedFacts.Concat(wrappedFact).ToArray();
            }
        }
    }
}