using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NRules.Diagnostics;

/// <summary>
/// Rete network graph that corresponds to the compiled rules.
/// </summary>
public class ReteGraph
{
    /// <summary>
    /// Creates an instance of a Rete network graph as a collection of nodes and links between them.
    /// </summary>
    /// <param name="nodes">Rete network graph nodes.</param>
    /// <param name="links">Links between Rete network nodes.</param>
    public ReteGraph(IEnumerable<ReteNode> nodes, IEnumerable<ReteLink> links)
    {
        Nodes = nodes.ToArray();
        Links = links.ToArray();
    }

    /// <summary>
    /// Nodes of the Rete network graph.
    /// </summary>
    public IReadOnlyCollection<ReteNode> Nodes { get; }

    /// <summary>
    /// Links between nodes of the Rete network graph.
    /// </summary>
    public IReadOnlyCollection<ReteLink> Links { get; }
}