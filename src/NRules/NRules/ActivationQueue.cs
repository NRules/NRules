using NRules.Collections;
using NRules.Utilities;

namespace NRules;

internal class ActivationQueue : ICanDeepClone<ActivationQueue>
{
    private readonly IPriorityQueue<int, Activation> _queue;

    public ActivationQueue()
        : this(new OrderedPriorityQueue<int, Activation>())
    {
    }

    private ActivationQueue(IPriorityQueue<int, Activation> queue)
    {
        _queue = queue;
    }

    public ActivationQueue DeepClone()
    {
        return new ActivationQueue(_queue.DeepClone());
    }

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
            if (current.IsEnqueued && current.Trigger.Matches(current.CompiledRule.ActionTriggers))
                return;
            Dequeue();
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

    public void CloneInto(ActivationQueue queue)
    {

    }
}