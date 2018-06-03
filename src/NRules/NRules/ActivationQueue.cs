using System;
using NRules.Collections;
using NRules.RuleModel;
using NRules.Utilities;

namespace NRules
{
    internal class ActivationQueue
    {
        private readonly IPriorityQueue<int, Activation> _queue = new OrderedPriorityQueue<int, Activation>();

        public void Enqueue(int priority, Activation activation)
        {
            if (!activation.IsEnqueued)
            {
                activation.IsEnqueued = true;
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
            activation.IsEnqueued = false;
            return activation;
        }

        public void Remove(Activation activation)
        {
            activation.IsEnqueued = false;
        }

        public bool HasActive()
        {
            PurgeQueue();
            return !_queue.IsEmpty;
        }

        private void PurgeQueue()
        {
            while (!_queue.IsEmpty)
            {
                Activation current = _queue.Peek();
                if (current.IsEnqueued && current.Trigger.Matches(current.CompiledRule.ActionTriggers)) return;
                _queue.Dequeue();
            }
        }

        public void Clear()
        {
            while (!_queue.IsEmpty)
            {
                Activation activation = Dequeue();
                activation.Clear();
            }
        }
    }
}