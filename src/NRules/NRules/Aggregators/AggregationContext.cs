using NRules.Rete;

namespace NRules.Aggregators
{
    /// <summary>
    /// Context associated with the aggregation operation.
    /// </summary>
    public class AggregationContext
    {
        internal IExecutionContext ExecutionContext { get; }
        internal NodeDebugInfo NodeInfo { get; }

        internal AggregationContext(IExecutionContext executionContext, NodeDebugInfo nodeInfo)
        {
            ExecutionContext = executionContext;
            NodeInfo = nodeInfo;
        }
    }
}
