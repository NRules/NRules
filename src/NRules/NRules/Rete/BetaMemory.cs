using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IBetaMemory
    {
        IList<Tuple> Tuples { get; }
    }

    internal class BetaMemory : IBetaMemory
    {
        private readonly List<Tuple> _tuples = new List<Tuple>();

        public IList<Tuple> Tuples
        {
            get { return _tuples; }
        }
    }
}