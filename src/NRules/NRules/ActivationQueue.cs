using System.Collections.Generic;
using NRules.Collections;
using NRules.Rete;

namespace NRules
{
    internal interface IActivationQueue
    {
        void Enqueue(int priority, Activation activation);
        Activation Dequeue();
        bool HasActive();
        void Remove(Activation activation);
    }

    internal class ActivationQueue : IActivationQueue
    {
        private readonly Dictionary<int, LinkedList<Activation>> _priorityBuckets = new Dictionary<int, LinkedList<Activation>>(); 

        private readonly IPriorityQueue<int, Activation> _queue = new OrderedPriorityQueue<int, Activation>();
        private readonly ISet<Activation> _activations = new HashSet<Activation>();

        public void Enqueue(int priority, Activation activation)
        {
            _activations.Add(activation);
            _queue.Enqueue(priority, activation);
        }

        public Activation Dequeue()
        {
            Activation activation = _queue.Dequeue();
            Remove(activation);
            return activation;
        }

        public void Remove(Activation activation)
        {
            _activations.Remove(activation);
        }

        public bool HasActive()
        {
            PurgeQueue();
            return QueueHasElements();
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