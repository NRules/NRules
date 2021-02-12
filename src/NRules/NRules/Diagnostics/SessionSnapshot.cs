using System;
using System.Collections.Generic;
using System.Linq;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Snapshot of rules session state.
    /// </summary>
    [Serializable]
    public class SessionSnapshot
    {
        private readonly NodeInfo[] _nodes;
        private readonly LinkInfo[] _links;
        
        internal SessionSnapshot(IEnumerable<NodeInfo> nodes, IEnumerable<LinkInfo> links)
        {
            _nodes = nodes.ToArray();
            _links = links.ToArray();
        }

        /// <summary>
        /// Nodes of the rete network graph.
        /// </summary>
        public NodeInfo[] Nodes => _nodes;

        /// <summary>
        /// Links between nodes of the rete network graph.
        /// </summary>
        public LinkInfo[] Links => _links;
    }
}