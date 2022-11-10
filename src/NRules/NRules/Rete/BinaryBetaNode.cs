using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Rete;

internal abstract class BinaryBetaNode : BetaNode, IObjectSink
{
    private static readonly IEnumerable<TupleFactSet> EmptySetList = Array.Empty<TupleFactSet>();
    private static readonly IReadOnlyDictionary<long, IEnumerable<Fact>> EmptyGroups = new Dictionary<long, IEnumerable<Fact>>();
    private readonly bool _isSubnetJoin;

    public ITupleSource LeftSource { get; }
    public IObjectSource RightSource { get; }

    protected BinaryBetaNode(int id, Type? outputType, ITupleSource leftSource, IObjectSource rightSource, bool isSubnetJoin)
        : base(id, outputType)
    {
        _isSubnetJoin = isSubnetJoin;
        LeftSource = leftSource;
        RightSource = rightSource;

        LeftSource.Attach(this);
        RightSource.Attach(this);
    }

    public abstract void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Fact> facts);
    public abstract void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Fact> facts);
    public abstract void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Fact> facts);

    protected TupleFactSet JoinedSet(IExecutionContext context, Tuple tuple)
    {
        var level = tuple.Level;
        var facts = RightSource.GetFacts(context).ToList();
        if (facts.Count == 0 || !_isSubnetJoin)
            return new TupleFactSet(tuple, facts);

        var factGroups = GroupFacts(facts, level);
        if (factGroups.Count == 0)
            return new TupleFactSet(tuple, facts);

        return JoinByGroupId(tuple, factGroups);
    }

    protected IEnumerable<TupleFactSet> JoinedSets(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        if (tuples.Count == 0)
            return EmptySetList;

        var facts = RightSource.GetFacts(context).ToList();
        if (facts.Count == 0 || !_isSubnetJoin)
            return CrossJoin(tuples, facts);

        var level = tuples.First().Level;
        var factGroups = GroupFacts(facts, level);
        if (factGroups.Count == 0)
            return CrossJoin(tuples, facts);

        return JoinByGroupId(tuples, factGroups);

    }

    protected IEnumerable<TupleFactSet> JoinedSets(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var tuples = LeftSource.GetTuples(context).ToList();
        if (tuples.Count == 0)
            return EmptySetList;

        if (!_isSubnetJoin)
            return CrossJoin(tuples, facts);

        var level = tuples[0].Level;
        var factGroups = GroupFacts(facts, level);
        if (factGroups.Count == 0)
            return CrossJoin(tuples, facts);

        return JoinByGroupId(tuples, factGroups);

    }

    private static IEnumerable<TupleFactSet> JoinByGroupId(IEnumerable<Tuple> tuples, IReadOnlyDictionary<long, IEnumerable<Fact>> factGroups)
    {
        return tuples.Select(tuple => JoinByGroupId(tuple, factGroups));
    }

    private static TupleFactSet JoinByGroupId(Tuple tuple, IReadOnlyDictionary<long, IEnumerable<Fact>> factGroups)
    {
        return factGroups.TryGetValue(tuple.Id, out var tupleFacts)
            ? new TupleFactSet(tuple, tupleFacts)
            : new TupleFactSet(tuple);
    }

    private static IEnumerable<TupleFactSet> CrossJoin(IEnumerable<Tuple> tuples, IReadOnlyCollection<Fact> facts)
    {
        return tuples.Select(tuple => new TupleFactSet(tuple, facts));
    }

    private static IReadOnlyDictionary<long, IEnumerable<Fact>> GroupFacts(IReadOnlyCollection<Fact> facts, int level)
    {
        if (facts.FirstOrDefault() is not WrapperFact)
            return EmptyGroups;

        return facts.OfType<WrapperFact>()
            .GroupBy(f => f.WrappedTuple.GetGroupId(level))
            .ToDictionary(g => g.Key, g => g.Cast<Fact>());
    }
}