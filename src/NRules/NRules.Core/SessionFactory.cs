using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rules;

namespace NRules.Core
{
    public class SessionFactory
    {
        private readonly IEnumerable<Rule> _rules;

        public SessionFactory(RuleRepository repository)
        {
            _rules = repository.Compile().ToArray();
        }

        public ISession CreateSession()
        {
            var session = new Session(_rules);
            return session;
        }
    }
}