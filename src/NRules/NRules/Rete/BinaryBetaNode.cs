using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Rete;

internal abstract class BinaryBetaNode : BetaNode, IObjectSink
{
    private readonly bool _isSubnetJoin;
    private static readonly IReadOnlyCollection<TupleFactSet> EmptySetList = Array.Empty<TupleFactSet>();
    private static readonly IReadOnlyDictionary<long, List<Fact>> EmptyGroups = new Dictionary<long, List<Fact>>();

    public ITupleSource LeftSource { get; }
    public IObjectSource RightSource { get; }

    protected BinaryBetaNode(ITupleSource leftSource, IObjectSource rightSource, bool isSubnetJoin)
    {
        _isSubnetJoin = isSubnetJoin;
        LeftSource = leftSource;
        RightSource = rightSource;

        LeftSource.Attach(this);
        RightSource.Attach(this);
    }

    public abstract void PropagateAssert(IExecutionContext context, List<Fact> facts);
    public abstract void PropagateUpdate(IExecutionContext context, List<Fact> facts);
    public abstract void PropagateRetract(IExecutionContext context, List<Fact> facts);

    protected TupleFactSet JoinedSet(IExecutionContext context, Tuple tuple)
    {
        int level = tuple.Level;
        var facts = RightSource.GetFacts(context).ToList();
        if (facts.Count > 0 && _isSubnetJoin)
        {
            var factGroups = GroupFacts(facts, level);
            if (factGroups.Count > 0)
                return JoinByGroupId(tuple, factGroups);
        }
        return new TupleFactSet(tuple, facts);
    }

    protected IEnumerable<TupleFactSet> JoinedSets(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        if (tuples.Count == 0)
            return EmptySetList;

        var facts = RightSource.GetFacts(context).ToList();
        if (facts.Count > 0 && _isSubnetJoin)
        {
            int level = tuples.First().Level;
            var factGroups = GroupFacts(facts, level);
            if (factGroups.Count > 0)
                return JoinByGroupId(tuples, factGroups);
        }

        return CrossJoin(tuples, facts);
    }

    protected IEnumerable<TupleFactSet> JoinedSets(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var tuples = LeftSource.GetTuples(context).ToList();
        if (tuples.Count == 0)
            return EmptySetList;

        if (_isSubnetJoin)
        {
            int level = tuples[0].Level;
            var factGroups = GroupFacts(facts, level);
            if (factGroups.Count > 0)
                return JoinByGroupId(tuples, factGroups);
        }

        return CrossJoin(tuples, facts);
    }

    private IEnumerable<TupleFactSet> JoinByGroupId(IEnumerable<Tuple> tuples, IReadOnlyDictionary<long, List<Fact>> factGroups)
    {
        var sets = new List<TupleFactSet>();
        foreach (var tuple in tuples)
        {
            var tupleFactSet = JoinByGroupId(tuple, factGroups);
            sets.Add(tupleFactSet);
        }
        return sets;
    }

    private static TupleFactSet JoinByGroupId(Tuple tuple, IReadOnlyDictionary<long, List<Fact>> factGroups)
    {
        var tupleFactSet = factGroups.TryGetValue(tuple.Id, out var tupleFacts)
            ? new TupleFactSet(tuple, tupleFacts)
            : new TupleFactSet(tuple, new List<Fact>());
        return tupleFactSet;
    }

    private IEnumerable<TupleFactSet> CrossJoin(IReadOnlyCollection<Tuple> tuples, IReadOnlyCollection<Fact> facts)
    {
        var sets = new List<TupleFactSet>(tuples.Count);
        foreach (var tuple in tuples)
        {
            sets.Add(new TupleFactSet(tuple, facts));
        }
        return sets;
    }

    private IReadOnlyDictionary<long, List<Fact>> GroupFacts(IReadOnlyCollection<Fact> facts, int level)
    {
        if (facts.Count == 0 || !facts.First().IsWrapperFact)
            return EmptyGroups;

        //This can be further optimized by grouping tuples by GroupId
        //and only descending to parent tuples once per group
        var factGroups = new Dictionary<long, List<Fact>>();
        foreach (var fact in facts)
        {
            var wrapperFact = (WrapperFact)fact;
            long groupId = wrapperFact.WrappedTuple.GetGroupId(level);
            if (!factGroups.TryGetValue(groupId, out var factGroup))
            {
                factGroup = new List<Fact>();
                factGroups[groupId] = factGroup;
            }
            factGroup.Add(wrapperFact);
        }
        return factGroups;
    }
}