using NRules.Rete;
using NRules.Utilities;

namespace NRules.Diagnostics;

/// <summary>
/// Provides access to performance metrics associated with individual nodes
/// in the Rete network used to execute the rules.
/// </summary>
public interface IMetricsProvider
{
    /// <summary>
    /// Retrieves performance metrics for a given Rete network node by the node id.
    /// </summary>
    /// <param name="nodeId">Id of the node for which to retrieve the metrics.</param>
    /// <returns>Rete network node performance metrics or <c>null</c>.</returns>
    INodeMetrics? FindByNodeId(int nodeId);

    /// <summary>
    /// Retries performance metrics for all nodes in the Rete network.
    /// </summary>
    /// <returns>Collection of Rete network node metrics.</returns>
    IEnumerable<INodeMetrics> GetAll();

    /// <summary>
    /// Resets cumulative performance metrics associated with all nodes in the network.
    /// </summary>
    void Reset();
}

internal interface IMetricsAggregator : IMetricsProvider
{
    NodeMetrics GetMetrics(INode node);
}

internal class MetricsAggregator : IMetricsAggregator
{
    private readonly Dictionary<int, NodeMetrics> _metrics = new();

    public INodeMetrics? FindByNodeId(int nodeId)
    {
        return _metrics.GetValueOrDefault(nodeId);
    }

    public IEnumerable<INodeMetrics> GetAll()
    {
        return _metrics.Values;
    }

    public NodeMetrics GetMetrics(INode node)
    {
        return _metrics.GetOrAdd(node.Id, id => new(id));
    }

    public void Reset()
    {
        foreach (var nodeMetrics in _metrics.Values)
        {
            nodeMetrics.Reset();
        }
    }
}
