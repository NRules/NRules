using System.Collections.Generic;
using System.Linq;

namespace NRules.Diagnostics;

/// <summary>
/// Rete network graph that corresponds to the compiled rules.
/// </summary>
public class ReteGraph
{
    /// <summary>
    /// Nodes of the Rete network graph.
    /// </summary>
    public IReadOnlyCollection<ReteNode> Nodes { get; }

    /// <summary>
    /// Links between nodes of the Rete network graph.
    /// </summary>
    public IReadOnlyCollection<ReteLink> Links { get; }
    
    internal ReteGraph(IEnumerable<ReteNode> nodes, IEnumerable<ReteLink> links)
    {
        Nodes = nodes.ToArray();
        Links = links.ToArray();
    }
}