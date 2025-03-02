using System.Collections.Generic;

namespace NRules.Collections;

internal class OrderedPriorityQueue<TPriority, TValue> : IPriorityQueue<TPriority, TValue>
{
    private readonly PriorityQueue<OrderedKey<TPriority>, TValue> _priorityQueue;
    private int _insertionOrderCounter = 0;

    public OrderedPriorityQueue()
        : this(Comparer<TPriority>.Default)
    {
    }

    public OrderedPriorityQueue(IComparer<TPriority> comparer)
    {
        var orderComparer = new OrderedKeyComparer<TPriority>(comparer);
        _priorityQueue = new PriorityQueue<OrderedKey<TPriority>, TValue>(orderComparer);
    }

    public void Enqueue(TPriority priority, TValue value)
    {
        var key = new OrderedKey<TPriority>(priority, _insertionOrderCounter++);
        _priorityQueue.Enqueue(key, value);
    }

    public TValue Dequeue()
    {
        return _priorityQueue.Dequeue();
    }

    public TValue Peek()
    {
        return _priorityQueue.Peek();
    }

    public bool IsEmpty => _priorityQueue.IsEmpty;

    public void Clear()
    {
        _priorityQueue.Clear();
    }

    private readonly struct OrderedKey<T>(T key, int order)
    {
        public T Key { get; } = key;
        public int Order { get; } = order;
    }

    private sealed class OrderedKeyComparer<T>(IComparer<T> comparer) : IComparer<OrderedKey<T>>
    {
        private readonly IComparer<int> _orderComparer = Comparer<int>.Default;

        public int Compare(OrderedKey<T> x, OrderedKey<T> y)
        {
            int result = comparer.Compare(x.Key, y.Key);
            if (result == 0)
            {
                result = -1*_orderComparer.Compare(x.Order, y.Order); //min first - FIFO
            }
            return result;
        }
    }
}