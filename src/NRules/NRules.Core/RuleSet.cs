using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Core
{
    public class RuleSet
    {
        private readonly EventHandler _eventHandler;
        private readonly IList<Type> _ruleTypes;

        internal RuleSet(IEnumerable<Type> ruleTypes, EventHandler eventHandler)
        {
            _eventHandler = eventHandler;
            _ruleTypes = ruleTypes.ToList();
        }

        public EventHandler EventHandler
        {
            get { return _eventHandler; }
        }

        public IEnumerable<Type> RuleTypes
        {
            get { return _ruleTypes; }
        }
    }
}