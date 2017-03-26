using System.Collections.Generic;

namespace NRules.Collections
{
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

        public bool IsEmpty
        {
            get { return _priorityQueue.IsEmpty; }
        }

        public void Clear()
        {
            _priorityQueue.Clear();
        }

        private class OrderedKey<T>
        {
            public T Key { get; private set; }
            public int Order { get; private set; }

            public OrderedKey(T key, int order)
            {
                Key = key;
                Order = order;
            }
        }

        private class OrderedKeyComparer<T> : IComparer<OrderedKey<T>>
        {
            private readonly IComparer<T> _keyComparer;
            private readonly IComparer<int> _orderComparer;

            public OrderedKeyComparer(IComparer<T> comparer)
            {
                _keyComparer = comparer;
                _orderComparer = Comparer<int>.Default;
            }

            public int Compare(OrderedKey<T> x, OrderedKey<T> y)
            {
                int result = _keyComparer.Compare(x.Key, y.Key);
                if (result == 0)
                {
                    result = -1*_orderComparer.Compare(x.Order, y.Order); //min first - FIFO
                }
                return result;
            }
        }
    }
}