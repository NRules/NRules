using System.Collections.Generic;

namespace NRules.Rete
{
    internal class FactResult : IFactResult
    {
        private readonly List<object> _failed;

        internal FactResult(List<object> failed)
        {
            _failed = failed;
        }

        public int FailedCount { get { return _failed.Count; } }
        public IEnumerable<object> Failed { get { return _failed; } }
    }
}