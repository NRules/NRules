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
        private readonly IAgenda _agenda;
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;

        public Session(INetwork network, IAgenda agenda, IWorkingMemory workingMemory)
        {
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
                RuleActivation activation = _agenda.NextActivation();
                var context = new ActionContext(_network, _workingMemory, activation.Tuple);

                foreach (IRuleAction action in activation.RuleDefinition.RightHandSide)
                {
                    action.Invoke(context);
                }
            }
        }
    }
}