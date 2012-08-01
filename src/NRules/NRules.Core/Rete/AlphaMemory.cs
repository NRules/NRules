using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface IAlphaMemory
    {
        IList<Fact> Facts { get; }
    }

    internal class AlphaMemory : IAlphaMemory
    {
        private readonly List<Fact> _facts = new List<Fact>();

        public IList<Fact> Facts
        {
            get { return _facts; }
        }
    }
}