using NRules.Collections;
using NRules.RuleModel;

namespace NRules
{
    internal class ActivationQueue
    {
        private readonly IPriorityQueue<int, Activation> _queue = new OrderedPriorityQueue<int, Activation>();

        public void Enqueue(int priority, Activation activation)
        {
            if (activation.CompiledRule.Repeatability == RuleRepeatability.NonRepeatable)
            {
                if (activation.IsRefracted)
                    return;
                activation.IsRefracted = true;
            }

            if (!activation.IsActive)
            {
                activation.IsActive = true;
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
            activation.IsActive = false;
            return activation;
        }

        public void Remove(Activation activation)
        {
            activation.IsActive = false;
            activation.IsRefracted = false;
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
                if (current.IsActive) return;
                _queue.Dequeue();
            }
        }

        public void Clear()
        {
            while (!_queue.IsEmpty)
            {
                Activation activation = Dequeue();
                activation.IsRefracted = false;
            }
        }
    }
}