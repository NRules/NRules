using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to error events raised during action execution.
    /// </summary>
    public class ActionErrorEventArgs : ErrorEventArgs
    {
        private readonly ICompiledRule _rule;
        private readonly Expression _expression;
        private readonly Tuple _tuple;

        internal ActionErrorEventArgs(Exception exception, ICompiledRule rule, Expression expression, Tuple tuple) : base(exception)
        {
            _rule = rule;
            _expression = expression;
            _tuple = tuple;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule { get { return _rule.Definition; } }

        /// <summary>
        /// Action that caused exception.
        /// </summary>
        public Expression Action { get { return _expression; } }

        /// <summary>
        /// Facts that caused exception.
        /// </summary>
        public IEnumerable<FactInfo> Facts { get { return _tuple.Facts.Reverse().Select(x => new FactInfo(x)).ToArray(); } }
    }
}