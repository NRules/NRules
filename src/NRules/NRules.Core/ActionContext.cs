using System;
using System.Linq;
using NRules.Core.Rete;
using Tuple = NRules.Core.Rete.Tuple;

namespace NRules.Core
{
    internal interface IActionContext
    {
        void Insert(object fact);
        void Update(object fact);
        void Retract(object fact);
        object Get(Type objectType);
    }

    internal class ActionContext : IActionContext
    {
        private readonly INetwork _network;
        private readonly IWorkingMemory _workingMemory;
        private readonly Tuple _tuple;

        public ActionContext(INetwork network, IWorkingMemory workingMemory, Tuple tuple)
        {
            _network = network;
            _workingMemory = workingMemory;
            _tuple = tuple;
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

        public object Get(Type objectType)
        {
            try
            {
                var arg = _tuple.Where(f => objectType.IsAssignableFrom(f.FactType)).Select(f => f.Object).First();
                return arg;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    string.Format("Could not get rule argument of requested type. Type={0}", objectType), e);
            }
        }
    }
}