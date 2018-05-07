using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to events raised during binding expression evaluation.
    /// </summary>
    public class BindingEventArgs : ExpressionEventArgs
    {
        private readonly ITuple _tuple;

        /// <summary>
        /// Initializes a new instance of the <c>BindingEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Binding expression arguments.</param>
        /// <param name="result">Binding expression result.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        public BindingEventArgs(Expression expression, Exception exception, object[] arguments, object result, ITuple tuple) 
            : base(expression, exception, arguments, result)
        {
            _tuple = tuple;
        }

        /// <summary>
        /// Facts related to the event.
        /// </summary>
        public IEnumerable<IFact> Facts => _tuple.OrderedFacts();
    }
}