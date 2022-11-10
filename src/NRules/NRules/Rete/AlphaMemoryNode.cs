using System;
using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal interface IAlphaMemoryNode : IObjectSource
{
    IEnumerable<IObjectSink> Sinks { get; }
}

internal class AlphaMemoryNode : IObjectSink, IAlphaMemoryNode
{
    private readonly List<IObjectSink> _sinks = new();

    public AlphaMemoryNode(int id, Type outputType)
    {
        Id = id;
        NodeInfo = new(outputType);
    }

    public int Id { get; set; }

    public NodeInfo NodeInfo { get; }

    public IEnumerable<IObjectSink> Sinks => _sinks;

    public void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var memory = context.WorkingMemory.GetNodeMemory(this);
        foreach (var sink in _sinks)
        {
            sink.PropagateAssert(context, facts);
        }

        using var counter = PerfCounter.Assert(context, this);
        memory.Add(facts);

        counter.AddItems(facts.Count);
        counter.SetCount(memory.FactCount);
    }

    public void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var memory = context.WorkingMemory.GetNodeMemory(this);
        var toUpdate = new List<Fact>();
        var toAssert = new List<Fact>();

        using (var counter = PerfCounter.Update(context, this))
        {
            foreach (var fact in facts)
            {
                if (memory.Contains(fact))
                    toUpdate.Add(fact);
                else
                    toAssert.Add(fact);
            }

            counter.AddInputs(facts.Count);
        }

        if (toUpdate.Count > 0)
        {
            foreach (var sink in _sinks)
            {
                sink.PropagateUpdate(context, toUpdate);
            }
        }
        if (toAssert.Count > 0)
        {
            PropagateAssert(context, toAssert);
        }
    }

    public void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Fact> facts)
    {
        var memory = context.WorkingMemory.GetNodeMemory(this);
        var toRetract = new List<Fact>();
        using (var counter = PerfCounter.Retract(context, this))
        {
            foreach (var fact in facts)
            {
                if (memory.Contains(fact))
                    toRetract.Add(fact);
            }

            counter.AddInputs(facts.Count);
            counter.AddOutputs(toRetract.Count);
        }

        if (toRetract.Count == 0)
            return;

        foreach (var sink in _sinks)
        {
            sink.PropagateRetract(context, toRetract);
        }

        using (var counter = PerfCounter.Retract(context, this))
        {
            memory.Remove(toRetract);
            counter.SetCount(memory.FactCount);
        }
    }

    public IEnumerable<Fact> GetFacts(IExecutionContext context)
    {
        var memory = context.WorkingMemory.GetNodeMemory(this);
        return memory.Facts;
    }

    public void Attach(IObjectSink sink)
    {
        _sinks.Add(sink);
    }

    public void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
    {
        visitor.VisitAlphaMemoryNode(context, this);
    }
}