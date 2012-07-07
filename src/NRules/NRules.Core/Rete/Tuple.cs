using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal class Tuple
    {
        private readonly List<Fact> _elements = new List<Fact>();

        public Tuple()
        {
        }

        public Tuple(Fact fact)
        {
            _elements.Add(fact);
        }

        public Tuple(Tuple left, Fact right)
        {
            _elements.AddRange(left.Elements);
            _elements.Add(right);
        }

        public IList<Fact> Elements
        {
            get { return _elements; }
        }
    }
}