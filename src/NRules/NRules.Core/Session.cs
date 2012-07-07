using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface ISession
    {
        void Insert(object fact);
        void Fire();
    }

    internal class Session : ISession
    {
        private readonly Network _network;
        private readonly Agenda _agenda;
        private readonly IDictionary<string, Rule> _ruleMap;

        public Session(IEnumerable<Rule> rules)
        {
            _ruleMap = rules.ToDictionary(r => r.Handle);

            var builder = new ReteBuilder();
            foreach (var rule in _ruleMap.Values)
            {
                builder.AddRule(rule);
            }

            _network = builder.GetNetwork();
            _agenda = new Agenda();
            _agenda.Subscribe(_network.EventSource);
        }

        public void Insert(object fact)
        {
            var internalFact = new Fact(fact);
            _network.PropagateAssert(internalFact);
        }

        public void Fire()
        {
            while (_agenda.ActivationQueue.Count > 0)
            {
                Activation activation = _agenda.ActivationQueue.Dequeue();
                var context = new ActionContext(activation.Tuple);

                Rule rule = _ruleMap[activation.RuleHandle];

                foreach (var action in rule.Actions)
                {
                    action.Invoke(context);
                }
            }
        }
    }
}