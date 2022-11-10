using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NRules.RuleModel;

namespace NRules.Rete;

[DebuggerDisplay("Tuple ({Count})")]
[DebuggerTypeProxy(typeof(TupleDebugView))]
internal sealed class Tuple : ITuple
{
    public Tuple(long id)
    {
        Id = id;
    }

    public Tuple(long id, Tuple left, Fact? right)
        : this(id)
    {
        RightFact = right;
        LeftTuple = left;
        Count = LeftTuple.Count + (RightFact is null ? 0 : 1);
        Level = LeftTuple.Level + 1;
    }

    public long Id { get; }
    public Fact? RightFact { get; }
    public Tuple? LeftTuple { get; }
    public int Count { get; }
    public int Level { get; }

    public long GetGroupId(int level)
    {
        if (LeftTuple is null)
        {
            return 0;
        }

        if (level == Level - 1)
        {
            return LeftTuple.Id;
        }

        return LeftTuple.GetGroupId(level);
    }

    /// <summary>
    /// Facts contained in the tuple in reverse order (fast iteration over linked list).
    /// Reverse collection to get facts in their actual order.
    /// </summary>
    IEnumerable<IFact> ITuple.Facts => new Enumerable(this);

    public IEnumerable<Fact> Facts
    {
        get
        {
            var tuple = this;
            while (tuple is not null && tuple.RightFact is Fact current)
            {
                yield return current;
                tuple = tuple.LeftTuple;
            }
        }
    }

    public Enumerator GetEnumerator() => new(this);

    internal class TupleDebugView
    {
        public readonly string Facts;

        public TupleDebugView(Tuple tuple)
        {
            var facts = string.Join(" || ", tuple.OrderedFacts().Select(f => f.Value));
            Facts = $"[{facts}]";
        }
    }

    internal class Enumerable : IEnumerable<Fact>, IEnumerator<Fact>
    {
        private readonly Tuple _tuple;
        private Enumerator _enumerator;

        public Enumerable(Tuple tuple)
        {
            _tuple = tuple;
            _enumerator = new Enumerator(tuple);
        }

        public IEnumerator<Fact> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose() { }
        public bool MoveNext() => _enumerator.MoveNext();
        public void Reset() => _enumerator = new Enumerator(_tuple);
        public Fact Current => _enumerator.Current ?? throw new InvalidOperationException($"{nameof(MoveNext)} was not called or enumeration ended");
        object IEnumerator.Current => Current;
    }

    internal struct Enumerator
    {
        private Tuple? _tuple;

        public Enumerator(Tuple tuple)
        {
            _tuple = tuple;
            Current = null;
        }

        public bool MoveNext()
        {
            do
            {
                Current = _tuple?.RightFact;
                _tuple = _tuple?.LeftTuple;
            } while (Current == null && _tuple != null);
            return Current != null;
        }

        public Fact? Current { get; private set; }
    }
}