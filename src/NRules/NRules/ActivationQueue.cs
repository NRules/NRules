using System.Collections.Generic;
using NRules.Collections;
using NRules.Rete;

namespace NRules
{
    internal class ActivationQueue
    {
        private readonly IPriorityQueue<int, Activation> _queue = new OrderedPriorityQueue<int, Activation>();
        private readonly ISet<Activation> _activations = new HashSet<Activation>();

        public void Enqueue(int priority, Activation activation)
        {
            if (_activations.Add(activation))
            {
                _queue.Enqueue(priority, activation);
            }
        }

        public Activation Dequeue()
        {
            Activation activation = _queue.Dequeue();
            return activation;
        }

        public void Remove(Activation activation)
        {
            _activations.Remove(activation);
        }

        public bool HasActive()
        {
            PurgeQueue();
            var hasActive = QueueHasElements();
            if (!hasActive) _activations.Clear();
            return hasActive;
        }

        private void PurgeQueue()
        {
            while (QueueHasElements() && IsCurrentQueueElementInactive())
            {
                _queue.Dequeue();
            }
        }

        private bool IsCurrentQueueElementInactive()
        {
            return !_activations.Contains(_queue.Peek());
        }

        private bool QueueHasElements()
        {
            return !_queue.IsEmpty;
        }
    }
}