using System.Collections.Generic;

namespace NRules.Utilities;

internal class ReverseComparer<T> : IComparer<T>
{
    private readonly IComparer<T> _comparer;

    internal ReverseComparer(IComparer<T> comparer)
    {
        _comparer = comparer;
    }

    public int Compare(T? x, T? y)
    {
#if NETSTANDARD
        return _comparer.Compare(y!, x!);
#else
        return _comparer.Compare(y, x);
#endif
    }
}
