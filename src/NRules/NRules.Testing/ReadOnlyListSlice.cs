using System.Collections;
using System.Collections.Generic;

namespace NRules.Testing;

internal class ReadOnlyListSlice<T> : IReadOnlyList<T>
{
    private readonly IReadOnlyList<T> _list;
    private readonly int _from;
    private readonly int _to;

    public ReadOnlyListSlice(IReadOnlyList<T> list, int from)
    {
        _list = list;
        _from = from;
        _to = list.Count;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = _from; i < _to; i++)
        {
            yield return _list[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public int Count => _to - _from;
    public T this[int index] => _list[_from + index];
}