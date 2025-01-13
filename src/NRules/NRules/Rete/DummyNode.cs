﻿using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal class DummyNode : IBetaMemoryNode
{
    private readonly List<ITupleSink> _sinks = new();

    public int Id { get; set; }
    public NodeInfo NodeInfo { get; } = new();
    public IReadOnlyCollection<ITupleSink> Sinks => _sinks;

    public void Activate(IExecutionContext context)
    {
        var tuple = new Tuple(context.IdGenerator.NextTupleId());
        var tupleList = new List<Tuple>();
        tupleList.Add(tuple);

        IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
        foreach (ITupleSink sink in _sinks)
        {
            sink.PropagateAssert(context, tupleList);
        }
        memory.Add(tupleList);
    }

    public IReadOnlyCollection<Tuple> GetTuples(IExecutionContext context)
    {
        IBetaMemory memory = context.WorkingMemory.GetNodeMemory(this);
        return memory.Tuples;
    }

    public void Attach(ITupleSink sink)
    {
        _sinks.Add(sink);
    }

    public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitDummyNode(context, this);
    }

    public void PropagateAssert(IExecutionContext context, TupleFactList tupleFactList)
    {
        //Do nothing
    }

    public void PropagateUpdate(IExecutionContext context, TupleFactList tupleFactList)
    {
        //Do nothing
    }

    public void PropagateRetract(IExecutionContext context, TupleFactList tupleFactList)
    {
        //Do nothing
    }
}