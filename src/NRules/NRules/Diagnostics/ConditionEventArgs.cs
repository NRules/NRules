using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NRules.Rete;
using NRules.RuleModel;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to events raised during condition evaluation.
    /// </summary>
    public class ConditionEventArgs : ExpressionEventArgs
    {
        private readonly ITuple _tuple;
        private readonly IFact _fact;

        /// <summary>
        /// Initializes a new instance of the <c>ConditionEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="arguments">Expression arguments.</param>
        /// <param name="result">Expression result.</param>
        /// <param name="tuple">Tuple related to the event.</param>
        /// <param name="fact">Fact related to the event.</param>
        public ConditionEventArgs(Expression expression, Exception exception, object[] arguments, object result, ITuple tuple, IFact fact)
            : base(expression, exception, arguments, result)
        {
            _tuple = tuple;
            _fact = fact;
        }

        /// <summary>
        /// Initializes a new instance of the <c>ConditionEventArgs</c> class.
        /// </summary>
        /// <param name="expression">Expression related to the event.</param>
        /// <param name="exception">Exception related to the event.</param>
        /// <param name="argument">Expression argument.</param>
        /// <param name="result">Expression result.</param>
        /// <param name="fact">Fact related to the event.</param>
        public ConditionEventArgs(Expression expression, Exception exception, object argument, object result, IFact fact)
            : base(expression, exception, argument, result)
        {
            _tuple = null;
            _fact = fact;
        }

        /// <summary>
        /// Facts related to the event.
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