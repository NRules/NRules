using System.Collections.Generic;
using NRules.Collections;
using NRules.RuleModel;

namespace NRules
{
    internal class ActivationQueue
    {
        private readonly IPriorityQueue<int, Activation> _queue = new OrderedPriorityQueue<int, Activation>();
        private readonly ISet<Activation> _activations = new HashSet<Activation>();
        private readonly ISet<Activation> _refractions = new HashSet<Activation>();

        public void Enqueue(int priority, Activation activation)
        {
            bool isRefracted = !_refractions.Add(activation);
            if (isRefracted && activation.CompiledRule.Repeatability == RuleRepeatability.NonRepeatable)
            {
                return;
            }

            bool isNewActivation = _activations.Add(activation);
            if (isNewActivation)
            {
                _queue.Enqueue(priority, activation);
            }
        }

        public Activation Peek()
        {
            Activation activation = _queue.Peek();
            return activation;
        }

        public Activation Dequeue()
        {
            Activation activation = _queue.Dequeue();
            _activations.Remove(activation);
            return activation;
        }

        public void Remove(Activation activation)
        {
            _activations.Remove(activation);
            _refractions.Remove(activation);
        }

        public bool HasActive()
        {
            PurgeQueue();
            var hasActive = QueueHasElements();
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

        public void Clear()
        {
            _queue.Clear();
            _activations.Clear();
            _refractions.Clear();
        }
    }
}