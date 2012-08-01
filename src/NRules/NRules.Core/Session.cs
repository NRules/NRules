using System.Collections.Generic;
using NRules.Core.Rete;
using NRules.Core.Rules;

namespace NRules.Core
{
    public interface ISession
    {
        void Insert(object fact);
        void Update(object fact);
        void Retract(object fact);
        void Fire();
    }

    internal class Session : ISession
    {
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;
        private readonly IDictionary<string, CompiledRule> _ruleMap;

        public Session(INetwork network, IAgenda agenda, IWorkingMemory workingMemory,
                       Dictionary<string, CompiledRule> ruleMap)
        {
            _network = network;
            _agenda = agenda;
            _workingMemory = workingMemory;
            _ruleMap = ruleMap;
        }

        public void Insert(object fact)
        {
            _network.PropagateAssert(_workingMemory, fact);
        }

        public void Update(object fact)
        {
            _network.PropagateUpdate(_workingMemory, fact);
        }

        public void Retract(object fact)
        {
            _network.PropagateRetract(_workingMemory, fact);
        }

        public void Fire()
        {
            while (_agenda.ActivationQueue.Count() > 0)
            {
                Activation activation = _agenda.ActivationQueue.Dequeue();
                var context = new ActionContext(_network, _workingMemory, activation.Tuple);

                CompiledRule rule = _ruleMap[activation.RuleHandle];

                foreach (IRuleAction action in rule.Actions)
                {
                    action.Invoke(context);
                }
            }
        }
    }
}