using NRules.Diagnostics;

namespace NRules.Aggregators;

/// <summary>
/// Context associated with the aggregation operation.
/// </summary>
public class AggregationContext
{
    /// <summary>
    /// Identity comparer for aggregated facts.
    /// </summary>
    public IFactIdentityComparer FactIdentityComparer => ExecutionContext.FactIdentityComparer;
    
    internal IExecutionContext ExecutionContext { get; }
    internal NodeInfo NodeInfo { get; }

    internal AggregationContext(IExecutionContext executionContext, NodeInfo nodeInfo)
    {
        ExecutionContext = executionContext;
        NodeInfo = nodeInfo;
    }
}
