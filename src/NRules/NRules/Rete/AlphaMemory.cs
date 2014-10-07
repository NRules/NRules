using System.Collections.Generic;
using NRules.Collections;

namespace NRules.Rete
{
    internal interface IAlphaMemory
    {
        IEnumerable<Fact> Facts { get; }
        bool Contains(Fact fact);
        void Add(Fact fact);
        void Remove(Fact fact);
    }

    internal class AlphaMemory : IAlphaMemory
    {
        private readonly OrderedHashSet<Fact> _facts = new OrderedHashSet<Fact>();

        public IEnumerable<Fact> Facts
        {
            get { return _facts; }
        }

        public bool Contains(Fact fact)
        {
            return _facts.Contains(fact);
        }

        public void Add(Fact fact)
        {
            _facts.Add(fact);
        }

        public void Remove(Fact fact)
        {
            _facts.Remove(fact);
        }
    }
}