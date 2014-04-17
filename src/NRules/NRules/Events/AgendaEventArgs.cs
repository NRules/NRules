using System;
using System.Collections.Generic;
using NRules.RuleModel;
using Tuple = NRules.Rete.Tuple;

namespace NRules.Events
{
    public class AgendaEventArgs : EventArgs
    {
        private readonly ICompiledRule _rule;
        private readonly Tuple _tuple;

        internal AgendaEventArgs(ICompiledRule rule, Tuple tuple)
        {
            _rule = rule;
            _tuple = tuple;
        }

        public IRuleDefinition Rule { get { return _rule.Definition; } }
        public IEnumerable<object> Facts { get { return _tuple.GetFactObjects(); } }
    }
}