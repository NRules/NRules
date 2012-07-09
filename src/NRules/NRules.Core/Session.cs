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
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IDictionary<string, Rule> _ruleMap;

        public Session(INetwork network, IAgenda agenda, Dictionary<string, Rule> ruleMap)
        {
            _network = network;
            _agenda = agenda;
            _ruleMap = ruleMap;
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

                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(context);
                }
            }
        }
    }
}