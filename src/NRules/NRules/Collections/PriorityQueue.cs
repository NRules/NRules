using System;
using System.Collections.Generic;

namespace NRules.Collections
{
    internal interface IPriorityQueue<in TPriority, TValue>
    {
        void Enqueue(TPriority priority, TValue value);
        TValue Dequeue();
        TValue Peek();
        bool IsEmpty { get; }
        void Clear();
    }

    internal class PriorityQueue<TPriority, TValue> : IPriorityQueue<TPriority, TValue>
    {
        private readonly List<KeyValuePair<TPriority, TValue>> _baseHeap;
        private readonly IComparer<TPriority> _comparer;

        public PriorityQueue()
            : this(Comparer<TPriority>.Default)
        {
        }

        public PriorityQueue(IComparer<TPriority> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _baseHeap = new List<KeyValuePair<TPriority, TValue>>();
            _comparer = comparer;
        }

        public void Enqueue(TPriority priority, TValue value)
        {
            Insert(priority, value);
        }

        public TValue Dequeue()
        {
            if (!IsEmpty)
            {
                var result = _baseHeap[0];
                DeleteRoot();
                return result.Value;
            }
            throw new InvalidOperationException("Priority queue is empty");
        }

        public TValue Peek()
        {
            if (!IsEmpty)
            {
                return _baseHeap[0].Value;
            }
            throw new InvalidOperationException("Priority queue is empty");
        }

        public bool IsEmpty
        {
            get { return _baseHeap.Count == 0; }
        }

        public void Clear()
        {
            _baseHeap.Clear();
        }

        private void ExchangeElements(int pos1, int pos2)
        {
            var val = _baseHeap[pos1];
            _baseHeap[pos1] = _baseHeap[pos2];
            _baseHeap[pos2] = val;
        }

        private void Insert(TPriority priority, TValue value)
        {
            var val = new KeyValuePair<TPriority, TValue>(priority, value);
            _baseHeap.Add(val);

            HeapifyUp();
        }

        private void HeapifyUp()
        {
            int pos = _baseHeap.Count - 1;

            while (pos > 0)
            {
                int parentPos = (pos - 1)/2;
                if (_comparer.Compare(_baseHeap[parentPos].Key, _baseHeap[pos].Key) < 0)
                {
                    ExchangeElements(parentPos, pos);
                    pos = parentPos;
                }
                else break;
            }
        }

        private void DeleteRoot()
        {
            if (_baseHeap.Count <= 1)
            {
                _baseHeap.Clear();
                return;
            }

            _baseHeap[0] = _baseHeap[_baseHeap.Count - 1];
            _baseHeap.RemoveAt(_baseHeap.Count - 1);

            HeapifyDown();
        }

        private void HeapifyDown()
        {
            int pos = 0;

            // heap[i] has children heap[2*i + 1] and heap[2*i + 2] and parent heap[(i-1)/ 2];

            while (true)
            {
                //exchange element with its largest child if heap property is violated
                int largest = pos;
                int left = 2*pos + 1;
                int right = 2*pos + 2;
                if (left < _baseHeap.Count && _comparer.Compare(_baseHeap[largest].Key, _baseHeap[left].Key) < 0)
                    largest = left;
                if (right < _baseHeap.Count && _comparer.Compare(_baseHeap[largest].Key, _baseHeap[right].Key) < 0)
                    largest = right;

                if (largest != pos)
                {
                    ExchangeElements(largest, pos);
                    pos = largest;
                }
                else break;
            }
        }
    }
}