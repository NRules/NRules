using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during condition evaluation.
    /// </summary>
    public class ConditionErrorEventArgs : ErrorEventArgs
    {
        private readonly ITuple _tuple;
        private readonly IFact _fact;

        /// <summary>
        /// Initializes a new instance of the <c>ConditionErrorEventArgs</c> class.
        /// </summary>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="expression">Condition expression related to the event.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        /// <param name="fact">Fact related to the event.</param>
        public ConditionErrorEventArgs(Exception exception, Expression expression, ITuple tuple, IFact fact)
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
                if (_tuple != null)
                {
                    foreach (var tupleFact in _tuple.OrderedFacts())
                    {
                        yield return tupleFact;
                    }
                }
                yield return _fact;
            }
        }
    }
}