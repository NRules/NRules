using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Events
{
    /// <summary>
    /// Information related to agenda events.
    /// </summary>
    public class AgendaEventArgs : EventArgs
    {
        private readonly ICompiledRule _rule;
        private readonly Tuple _tuple;

        internal AgendaEventArgs(ICompiledRule rule, Tuple tuple)
        {
            _rule = rule;
            _tuple = tuple;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule { get { return _rule.Definition; } }

        /// <summary>
        /// Tuple related to the event.
        /// </summary>
        public IEnumerable<FactInfo> Facts { get { return _tuple.Facts.Reverse().Select(t => new FactInfo(t)).ToArray(); } }
    }
}