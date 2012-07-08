using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
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
            //todo: use di container
            var session = new Session(new ReteBuilder(), new Agenda());
            session.SetRules(_rules);
            return session;
        }
    }
}