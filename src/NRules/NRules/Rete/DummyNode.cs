using NRules.Diagnostics;

namespace NRules.Rete;

internal sealed class DummyNode : IBetaMemoryNode
{
    private readonly List<ITupleSink> _sinks = new();

    public DummyNode(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public NodeInfo NodeInfo { get; } = new();
    public IEnumerable<ITupleSink> Sinks => _sinks;

    public void Activate(IExecutionContext context)
    {
        var tuple = new Tuple(context.IdGenerator.NextTupleId());
        var tupleList = new[] { tuple };

        var memory = context.WorkingMemory.GetNodeMemory(this);
        foreach (var sink in _sinks)
        {
            sink.PropagateAssert(context, tupleList);
        }
        memory.Add(tupleList);
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