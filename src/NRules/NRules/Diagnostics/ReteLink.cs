namespace NRules.Diagnostics
{
    /// <summary>
    /// Link between nodes in the rete network graph.
    /// </summary>
    public class ReteLink
    {
        /// <summary>
        /// Source node.
        /// </summary>
        public ReteNode Source { get; }

        /// <summary>
        /// Target node.
        /// </summary>
        public ReteNode Target { get; }

        internal ReteLink(ReteNode source, ReteNode target)
        {
            Source = source;
            Target = target;
        }
    }
}