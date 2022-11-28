using System.Collections.Generic;
using NRules.Diagnostics;
using NRules.Utilities;

namespace NRules.Rete;

internal class DummyNode : IBetaMemoryNode, ICanDeepClone<DummyNode>
{
    private readonly List<ITupleSink> _sinks = new();

    public int Id { get; set; }
    public NodeInfo NodeInfo { get; } = new NodeInfo();
    public IEnumerable<ITupleSink> Sinks => _sinks;

    public DummyNode DeepClone()
    {
        var node = new DummyNode { Id = Id };
        NodeInfo.CloneInto(node.NodeInfo);
        _sinks.CloneInto(node._sinks);
        return node;
    }

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

    public IEnumerable<Tuple> GetTuples(IExecutionContext context)
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