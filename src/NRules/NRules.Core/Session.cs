using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using NRules.Rule;

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
        private readonly IDictionary<string, IRuleDefinition> _ruleMap;
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;

        public Session(IEnumerable<IRuleDefinition> rules, INetwork network, IAgenda agenda, IWorkingMemory workingMemory)
        {
            _ruleMap = rules.ToDictionary(rule => rule.Handle);
            _network = network;
            _agenda = agenda;
            _workingMemory = workingMemory;
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
            while (_agenda.HasActiveRules())
            {
                Activation activation = _agenda.NextActivation();
                IRuleDefinition ruleDefinition = _ruleMap[activation.RuleHandle];
                var context = new ActionContext(_network, _workingMemory, activation.Tuple);

                foreach (IRuleAction action in ruleDefinition.RightHandSide)
                {
                    action.Invoke(context);
                }
            }
        }
    }
}