using System.Collections.Generic;
using NRules.Core.Rete;

namespace NRules.Core
{
    internal class ActivationQueue
    {
        private readonly LinkedList<Activation> _activations = new LinkedList<Activation>();

        public void Enqueue(Activation activation)
        {
            _activations.AddLast(activation);
        }

        public Activation Dequeue()
        {
            Activation activation = _activations.First.Value;
            _activations.RemoveFirst();
            return activation;
        }

        public int Count()
        {
            return _activations.Count;
        }

        public void Remove(Activation activation)
        {
            _activations.Remove(activation);
        }
    }
}