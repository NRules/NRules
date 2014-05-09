using System;
using System.Collections.Generic;

namespace NRules.Diagnostics
{
    [Serializable]
    public class SessionSnapshot
    {
        private readonly List<NodeInfo> _nodes;
        private readonly List<LinkInfo> _links;
        
        internal SessionSnapshot(IEnumerable<NodeInfo> nodes, IEnumerable<LinkInfo> links)
        {
            _nodes = new List<NodeInfo>(nodes);
            _links = new List<LinkInfo>(links);
        }

        public IEnumerable<NodeInfo> Nodes
        {
            get { return _nodes; }
        }

        public IEnumerable<LinkInfo> Links
        {
            get { return _links; }
        }
    }
}