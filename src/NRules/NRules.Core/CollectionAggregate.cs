using System;
using System.Collections.Generic;
using NRules.Core.Rules;

namespace NRules.Core
{
    internal class CollectionAggregate<T> : IAggregate
    {
        private readonly List<T> _items = new List<T>();
        private bool _initialized;

        public void Add(params object[] facts)
        {
            _items.Add((T) facts[0]);
            if (!_initialized)
            {
                _initialized = true;
                ResultAdded(_items, EventArgs.Empty);
            }
            else
            {
                ResultModified(_items, EventArgs.Empty);
            }
        }

        public void Modify(params object[] facts)
        {
            ResultModified(_items, EventArgs.Empty);
        }

        public void Remove(params object[] facts)
        {
            _items.Remove((T) facts[0]);
            if (_items.Count > 0)
            {
                ResultModified(_items, EventArgs.Empty);
            }
            else
            {
                ResultRemoved(_items, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> ResultAdded;
        public event EventHandler<EventArgs> ResultModified;
        public event EventHandler<EventArgs> ResultRemoved;
    }
}