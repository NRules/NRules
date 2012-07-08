using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly IReteBuilder _reteBuilder;
        private readonly IAgenda _agenda;
        private INetwork _network;
        private IDictionary<string, Rule> _ruleMap;

        //todo: possibly move session setup to factory.
        public bool IsConfigured { get; private set; }

        public Session(IReteBuilder reteBuilder, IAgenda agenda)
        {
            _reteBuilder = reteBuilder;
            _agenda = agenda;
            IsConfigured = false;
        }

        public void SetRules(IEnumerable<Rule> rules)
        {
            if(IsConfigured)
                throw new ApplicationException("This session has already been configured with a set of " + 
                                               "rules and cannot be configured with another.");

            IsConfigured = true;
            _ruleMap = rules.ToDictionary(r => r.Handle);

            foreach (Rule rule in _ruleMap.Values)
            {
                _reteBuilder.AddRule(rule);
            }

            _network = _reteBuilder.GetNetwork();
            _agenda.Subscribe(_network.EventSource);
        }

        public void Insert(object fact)
        {
            AssertIsConfigured();
            var internalFact = new Fact(fact);
            _network.PropagateAssert(internalFact);
        }

        public void Fire()
        {
            AssertIsConfigured();

            while (_agenda.ActivationQueue.Count > 0)
            {
                Activation activation = _agenda.ActivationQueue.Dequeue();
                var context = new ActionContext(activation.Tuple);

                Rule rule = _ruleMap[activation.RuleHandle];

                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(context);
                }
            }
        }

        private void AssertIsConfigured()
        {
            if (!IsConfigured)
                throw new ApplicationException("This session has not been configured with a set of " +
                                               "rules and must be before this action can be performed.");
        }
    }
}