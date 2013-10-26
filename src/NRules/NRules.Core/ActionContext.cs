using NRules.Core.Rete;

namespace NRules.Core
{
    public interface IActionContext
    {
        void Insert(object fact);
        void Update(object fact);
        void Retract(object fact);
        void Halt();
    }

    internal class ActionContext : IActionContext
    {
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;

        public ActionContext(INetwork network, IWorkingMemory workingMemory)
        {
            _network = network;
            _workingMemory = workingMemory;
            IsHalted = false;
        }

        public bool IsHalted { get; set; }

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

        public void Halt()
        {
            IsHalted = true;
        }
    }
}