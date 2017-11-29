using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during binding expression evaluation.
    /// </summary>
    public class BindingErrorEventArgs : ErrorEventArgs
    {
        private readonly Tuple _tuple;

        internal BindingErrorEventArgs(Exception exception, Expression expression, Tuple tuple) : base(exception)
        {
            _tuple = tuple;
            Expression = expression;
        }

        /// <summary>
        /// Expression that caused exception.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFact> Facts => _tuple.OrderedFacts();
    }
}