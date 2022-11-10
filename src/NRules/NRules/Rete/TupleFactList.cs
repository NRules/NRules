using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete;

[DebuggerDisplay("TupleFactList ({Count})")]
internal class TupleFactList : IReadOnlyCollection<(Tuple Tuple, Fact? Fact)>
{
    private readonly List<(Tuple Tuple, Fact? Fact)> _list = new();

    public int Count => _list.Count;

    public void Add(Tuple tuple, Fact? fact = null)
    {
        _list.Add((tuple, fact));
    }

    public IEnumerator<(Tuple Tuple, Fact? Fact)> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}
