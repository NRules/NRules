using System;
using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal abstract class BetaNode : ITupleSink
{
    protected BetaNode(int id, Type? outputType = null)
    {
        Id = id;
        NodeInfo = new(outputType);
    }

    public int Id { get; }
    public NodeInfo NodeInfo { get; }
    public IBetaMemoryNode? MemoryNode { get; set; }

    public abstract void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
    public abstract void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
    public abstract void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);

    public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
}