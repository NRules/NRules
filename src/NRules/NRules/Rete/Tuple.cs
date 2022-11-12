using System.Collections;
using System.Diagnostics;
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

    public Tuple(long id, Tuple parent, Fact? fact = null)
        : this(id)
    {
        Fact = fact;
        Parent = parent;
        Count = Parent.Count + (Fact is null ? 0 : 1);
        Level = Parent.Level + 1;
    }

    public long Id { get; }
    public Fact? Fact { get; }
    public Tuple? Parent { get; }
    public int Count { get; }
    public int Level { get; }

    public Tuple CreateChild(long id, Fact? fact = null) => new(id, this, fact);

    public long GetGroupId(int level)
    {
        if (Parent is null)
        {
            return 0;
        }

        if (level == Level - 1)
        {
            return Parent.Id;
        }

        return Parent.GetGroupId(level);
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
            while (tuple is not null)
            {
                var next = NextTupleWithFact(tuple);
                if (next is null)
                {
                    yield break;
                }

                var current = next.Fact;
                if (current == null)
                {
                    yield break;
                }

                tuple = next.Parent;
                yield return current;
            }
            static Tuple? NextTupleWithFact(Tuple? tuple)
            {
                while (tuple is not null && tuple.Fact is null)
                {
                    tuple = tuple.Parent;
                }
                return tuple;
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
            Current = null!;
        }

        public bool MoveNext()
        {
            do
            {
                Current = _tuple?.Fact!;
                _tuple = _tuple?.Parent;
            } while (Current == null && _tuple != null);
            return Current != null;
        }

        public Fact Current { get; private set; }
    }
}