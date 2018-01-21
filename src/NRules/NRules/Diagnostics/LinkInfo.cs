namespace NRules.Diagnostics
{
    /// <summary>
    /// Link between nodes in the rete network graph.
    /// </summary>
#if (NET45 || NETSTANDARD2_0)
    [System.Serializable]
#endif
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