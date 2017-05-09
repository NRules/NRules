using System.Collections.Generic;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Snapshot of rules session state.
    /// </summary>
#if NET45
    [System.Serializable]
#endif
    public class SessionSnapshot
    {
        private readonly List<NodeInfo> _nodes;
        private readonly List<LinkInfo> _links;
        
        internal SessionSnapshot(IEnumerable<NodeInfo> nodes, IEnumerable<LinkInfo> links)
        {
            _nodes = new List<NodeInfo>(nodes);
            _links = new List<LinkInfo>(links);
        }

        /// <summary>
        /// Nodes of the rete network graph.
        /// </summary>
        public IEnumerable<NodeInfo> Nodes
        {
            get { return _nodes; }
        }

        /// <summary>
        /// Links between nodes of the rete network graph.
        /// </summary>
        public IEnumerable<LinkInfo> Links
        {
            get { return _links; }
        }
    }
}