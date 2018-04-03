using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during aggregate expression evaluation.
    /// </summary>
    public class AggregateErrorEventArgs : ErrorEventArgs
    {
        private readonly ITuple _tuple;
        private readonly IFact _fact;

        /// <summary>
        /// Initializes a new instance of the <c>AggregateErrorEventArgs</c> class.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="expression">Aggregate expression related to the event.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        /// <param name="fact">Fact related to the event.</param>
        public AggregateErrorEventArgs(Exception exception, Expression expression, ITuple tuple, IFact fact) : base(exception)
        {
            _tuple = tuple;
            _fact = fact;
            Expression = expression;
        }

        /// <summary>
        /// Expression that caused exception.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<IFact> Facts
        {
            get
            {
                foreach (var tupleFact in _tuple.OrderedFacts())
                {
                    yield return tupleFact;
                }
                yield return _fact;
            }
        }

    }
}