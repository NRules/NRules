using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during binding expression evaluation.
    /// </summary>
    public class BindingErrorEventArgs : ErrorEventArgs
    {
        private readonly ITuple _tuple;

        /// <summary>
        /// Initializes a new instance of the <c>BindingErrorEventArgs</c> class.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="expression">Binding expression related to the event.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        public BindingErrorEventArgs(Exception exception, Expression expression, ITuple tuple) : base(exception)
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