using System.Collections.Generic;

namespace NRules.Collections;

internal class OrderedPriorityQueue<TPriority, TValue> : IPriorityQueue<TPriority, TValue>
{
    private readonly IPriorityQueue<OrderedKey<TPriority>, TValue> _priorityQueue;
    private int _insertionOrderCounter = 0;

    public OrderedPriorityQueue()
        : this(Comparer<TPriority>.Default)
    {
    }

    public OrderedPriorityQueue(IComparer<TPriority> comparer)
        : this(new PriorityQueue<OrderedKey<TPriority>, TValue>(new OrderedKeyComparer<TPriority>(comparer)))
    {
    }

    private OrderedPriorityQueue(IPriorityQueue<OrderedKey<TPriority>, TValue> priorityQueue)
    {
        _priorityQueue = priorityQueue;
    }

    public IPriorityQueue<TPriority, TValue> DeepClone() =>
        new OrderedPriorityQueue<TPriority, TValue>(_priorityQueue.DeepClone())
        {
            _insertionOrderCounter = _insertionOrderCounter
        };

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

    private readonly struct OrderedKey<T>
    {
        public T Key { get; }
        public int Order { get; }

        public OrderedKey(T key, int order)
        {
            Key = key;
            Order = order;
        }
    }

    private class OrderedKeyComparer<T> : IComparer<OrderedKey<T>>
    {
        private readonly IComparer<T> _keyComparer;
        private readonly IComparer<int> _orderComparer = Comparer<int>.Default;

        public OrderedKeyComparer(IComparer<T> comparer)
        {
            _keyComparer = comparer;
        }

        public int Compare(OrderedKey<T> x, OrderedKey<T> y)
        {
            int result = _keyComparer.Compare(x.Key, y.Key);
            if (result == 0)
            {
                result = -1 * _orderComparer.Compare(x.Order, y.Order); //min first - FIFO
            }
            return result;
        }
    }
}