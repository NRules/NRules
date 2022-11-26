using System;
using System.Collections.Generic;
using NRules.Diagnostics;

namespace NRules.Rete;

internal abstract class BetaNode : ITupleSink
{
    private BetaMemoryNode? _memoryNode;

    public int Id { get; set; }
    public NodeInfo NodeInfo { get; } = new();

    public abstract void PropagateAssert(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
    public abstract void PropagateUpdate(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);
    public abstract void PropagateRetract(IExecutionContext context, IReadOnlyCollection<Tuple> tuples);

    public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);

    public IBetaMemoryNode EnsureMemoryNodeInitialized(Func<int> getNodeIdFunc)
    {
        return _memoryNode ??= new BetaMemoryNode { Id = getNodeIdFunc() };
    }

    internal protected IBetaMemoryNode EnsureMemoryNode()
    {
        return _memoryNode ?? throw new InvalidOperationException($"{nameof(_memoryNode)} as not initialized");
    }
}