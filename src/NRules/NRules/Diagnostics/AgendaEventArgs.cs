using System;
using System.Collections.Generic;
using System.Linq;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Information related to agenda events.
    /// </summary>
    public class AgendaEventArgs : EventArgs
    {
        private readonly ICompiledRule _compiledRule;
        private readonly Tuple _tuple;

        internal AgendaEventArgs(ICompiledRule compiledRule, Tuple tuple)
        {
            _compiledRule = compiledRule;
            _tuple = tuple;
        }

        /// <summary>
        /// Rule related to the event.
        /// </summary>
        public IRuleDefinition Rule { get { return _compiledRule.Definition; } }

        /// <summary>
        /// Tuple related to the event.
        /// </summary>
        public IEnumerable<FactInfo> Facts
        {
            get { return _tuple.OrderedFacts.Select(f => new FactInfo(f)).ToArray(); }
        }
    }
}
