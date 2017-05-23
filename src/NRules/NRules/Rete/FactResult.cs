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

        public int FailedCount => _failed.Count;
        public IEnumerable<object> Failed => _failed;
    }
}