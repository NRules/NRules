using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Fluent;
using NRules.Fluent.Dsl;

namespace NRules.Testing
{
    public class CachedRuleActivator : IRuleActivator
    {
        private readonly IRuleActivator _activator;
        private readonly Dictionary<Type, IEnumerable<Rule>> _rules = new();

        public CachedRuleActivator(IRuleActivator activator)
        {
            _activator = activator;
        }

        public IEnumerable<Rule> Activate(Type type)
        {
            if (_rules.TryGetValue(type, out var rules))
            {
                return rules;
            }

            return _rules[type] = _activator.Activate(type).ToArray();
        }
    }
}
