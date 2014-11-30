using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.Rete;
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
            _tuple = tuple;
            _fact = fact;
            Condition = expression;
        }

        /// <summary>
        /// Condition that caused exception.
        /// </summary>
        public Expression Condition { get; private set; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<FactInfo> Facts
        {
            get
            {
                var wrappedFact = new[] { new FactInfo(_fact) };
                return _tuple == null
                    ? wrappedFact
                    : _tuple.Facts.Reverse().Select(x => new FactInfo(x)).Concat(wrappedFact).ToArray();
            }
        }
    }
}