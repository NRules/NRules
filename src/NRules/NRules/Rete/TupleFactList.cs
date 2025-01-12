using System.Collections.Generic;
using System.Diagnostics;

namespace NRules.Rete;

[DebuggerDisplay("TupleFactList ({Count})")]
internal class TupleFactList
{
    private readonly List<TupleItem> _tuples = new(); 
    private readonly List<Fact?> _facts = new();

    public int Count => _tuples.Count;

    public void Add(Tuple tuple)
    {
        AddTuple(tuple);
        _facts.Add(null);
    }

    public void Add(Tuple tuple, Fact fact)
    {
        AddTuple(tuple);
        _facts.Add(fact);
    }

    private void AddTuple(Tuple tuple)
    {
        if (_tuples.Count > 0 &&
            _tuples[_tuples.Count - 1].Tuple == tuple)
            _tuples[_tuples.Count - 1].Count++;
        else
            _tuples.Add(new TupleItem(tuple));
    }

    public class TupleItem(Tuple tuple)
    {
        public Tuple Tuple { get; } = tuple;
        public int Count { get; set; } = 1;
    }

    public struct Enumerator(List<TupleItem> tuples, List<Fact?> facts)
    {
        private int _factIndex = -1;
        private int _tupleIndex = -1;
        private int _tupleItemIndex = 0;

        public Tuple CurrentTuple => tuples[_tupleIndex].Tuple;
        public Fact? CurrentFact => facts[_factIndex];

        public bool MoveNext()
        {
            _factIndex++;
            if (_factIndex >= facts.Count)
                return false;

            if (_tupleIndex < 0 || _tupleItemIndex == tuples[_tupleIndex].Count - 1)
            {
                _tupleIndex++;
                _tupleItemIndex = 0;
            }
            else
            {
                _tupleItemIndex++;
            }

            return true;
        }
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(_tuples, _facts);
    }
}
