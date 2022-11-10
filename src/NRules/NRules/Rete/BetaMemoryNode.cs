using System;
using System.Collections.Generic;
using System.Linq;
using NRules.Diagnostics;

namespace NRules.Rete;

internal interface IBetaMemoryNode : ITupleSource, INode
{
    IEnumerable<ITupleSink> Sinks { get; }
    void PropagateAssert(IExecutionContext context, TupleFactList tupleFactList);
    void PropagateUpdate(IExecutionContext context, TupleFactList tupleFactList);
    void PropagateRetract(IExecutionContext context, TupleFactList tupleFactList);
}

internal class BetaMemoryNode : IBetaMemoryNode
{
    private readonly List<ITupleSink> _sinks = new();

    public BetaMemoryNode(int id)
    {
        Id = id;
    }

    public int Id { get; }
    public NodeInfo NodeInfo { get; } = new();
    public IEnumerable<ITupleSink> Sinks => _sinks;

    public void PropagateAssert(IExecutionContext context, TupleFactList tupleFactList)
    {
        if (tupleFactList.Count == 0)
        {
            return;
        }

        var memory = context.WorkingMemory.GetNodeMemory(this);
        var toAssert = new List<Tuple>();

        using (var counter = PerfCounter.Assert(context, this))
        {
            foreach (var (tuple, fact) in tupleFactList)
            {
                var childTuple = new Tuple(context.IdGenerator.NextTupleId(), tuple, fact);
                toAssert.Add(childTuple);
            }

            counter.AddItems(tupleFactList.Count);
        }

        PropagateAssertInternal(context, memory, toAssert);
    }

    public void PropagateUpdate(IExecutionContext context, TupleFactList tupleFactList)
    {
        if (tupleFactList.Count == 0)
        {
            return;
        }

        var memory = context.WorkingMemory.GetNodeMemory(this);
        var toAssert = new List<Tuple>();
        var toUpdate = new List<Tuple>();

        using (var counter = PerfCounter.Update(context, this))
        {
            foreach (var (tuple, fact) in tupleFactList)
            {
                var childTuple = memory.FindTuple(tuple, fact);
                if (childTuple == null)
                {
                    childTuple = new Tuple(context.IdGenerator.NextTupleId(), tuple, fact);
                    toAssert.Add(childTuple);
                }
                else
                {
                    toUpdate.Add(childTuple);
                }
            }

            counter.AddItems(tupleFactList.Count);
        }

        PropagateAssertInternal(context, memory, toAssert);
        PropagateUpdateInternal(context, toUpdate);
    }

    public void PropagateRetract(IExecutionContext context, TupleFactList tupleFactList)
    {
        if (tupleFactList.Count == 0)
        {
            return;
        }

        var memory = context.WorkingMemory.GetNodeMemory(this);
        var toRetract = new List<Tuple>();

        using (var counter = PerfCounter.Retract(context, this))
        {
            foreach (var (tuple, fact) in tupleFactList)
            {
                var childTuple = memory.FindTuple(tuple, fact);
                if (childTuple != null)
                {
                    toRetract.Add(childTuple);
                }
            }

            counter.AddInputs(tupleFactList.Count);
            counter.AddOutputs(toRetract.Count);
        }

        PropagateRetractInternal(context, memory, toRetract);
    }

    private void PropagateAssertInternal(IExecutionContext context, IBetaMemory memory, IReadOnlyCollection<Tuple> tuples)
    {
        if (tuples.Count == 0)
        {
            return;
        }

        foreach (var sink in _sinks)
        {
            sink.PropagateAssert(context, tuples);
        }

        using var counter = PerfCounter.Assert(context, this);
        memory.Add(tuples);
        counter.SetCount(memory.Tuples.Count);
    }

    private void PropagateUpdateInternal(IExecutionContext context, IReadOnlyCollection<Tuple> tuples)
    {
        if (tuples.Count <= 0)
        {
            return;
        }

        foreach (var sink in _sinks)
        {
            sink.PropagateUpdate(context, tuples);
        }
    }

    private void PropagateRetractInternal(IExecutionContext context, IBetaMemory memory, IReadOnlyCollection<Tuple> tuples)
    {
        if (tuples.Count <= 0)
        {
            return;
        }

        using (var counter = PerfCounter.Retract(context, this))
        {
            memory.Remove(tuples);
            counter.SetCount(memory.Tuples.Count);
        }

        foreach (var sink in Sinks.Reverse())
        {
            sink.PropagateRetract(context, tuples);
        }
    }

    public IEnumerable<Tuple> GetTuples(IExecutionContext context)
    {
        var memory = context.WorkingMemory.GetNodeMemory(this);
        return memory.Tuples;
    }

    public void Attach(ITupleSink sink)
    {
        _sinks.Add(sink);
    }

    public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitBetaMemoryNode(context, this);
    }
}