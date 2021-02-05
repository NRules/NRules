using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IAlphaMemory
    {
        IEnumerable<Fact> Facts { get; }
        bool Contains(Fact fact);
        void Add(List<Fact> facts);
        void Remove(List<Fact> facts);
    }

    internal class AlphaMemory : IAlphaMemory
    {
        private readonly HashSet<Fact> _facts = new HashSet<Fact>();

        public IEnumerable<Fact> Facts => _facts;

        public bool Contains(Fact fact)
        {
            return _facts.Contains(fact);
        }

        public void Add(List<Fact> facts)
        {
            foreach (var fact in facts)
            {
                _facts.Add(fact);
            }
        }

        public void Remove(List<Fact> facts)
        {
            foreach (var fact in facts)
            {
                _facts.Remove(fact);
            }
        }
    }
}
