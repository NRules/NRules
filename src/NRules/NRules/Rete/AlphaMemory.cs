using System.Collections.Generic;
using NRules.Collections;

namespace NRules.Rete
{
    internal interface IAlphaMemory
    {
        IEnumerable<Fact> Facts { get; }
        bool Contains(Fact fact);
        void Add(IEnumerable<Fact> facts);
        void Remove(IEnumerable<Fact> facts);
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

        public void Add(IEnumerable<Fact> facts)
        {
            foreach (var fact in facts)
                Add(fact);
        }

        public void Remove(Fact fact)
        {
            _facts.Remove(fact);
        }

        public void Remove(IEnumerable<Fact> facts)
        {
            foreach (var fact in facts)
                Remove(fact);
        }
    }
}
