using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Core.Rete;
using Tuple = NRules.Core.Rete.Tuple;

namespace NRules.Core
{
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

        public T Arg<T>()
        {
            try
            {
                var arg = _tuple.Where(f => f.FactType == typeof (T)).Select(f => f.Object).Cast<T>().First();
                return arg;
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException(
                    string.Format("Could not get rule argument of type {0}", typeof (T)), e);
            }
        }

        public IEnumerable<T> Collection<T>()
        {
            return Arg<IEnumerable<T>>();
        }
    }
}