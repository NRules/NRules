using System;

namespace NRules.Diagnostics
{
    /// <summary>
    /// Link between nodes in the rete network graph.
    /// </summary>
    [Serializable]
    public class LinkInfo
    {
        /// <summary>
        /// Source node.
        /// </summary>
        public NodeInfo Source { get; }

        /// <summary>
        /// Target node.
        /// </summary>
        public NodeInfo Target { get; }

        internal LinkInfo(NodeInfo source, NodeInfo target)
        {
            Source = source;
            Target = target;
        }
    }
}