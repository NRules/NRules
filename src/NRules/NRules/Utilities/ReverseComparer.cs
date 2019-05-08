using System.Collections.Generic;

namespace NRules.Utilities
{
    internal class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _comparer;

        internal ReverseComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer.Compare(y, x);
        }
    }
}
