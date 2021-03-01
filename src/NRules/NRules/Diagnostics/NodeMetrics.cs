namespace NRules.Diagnostics
{
    /// <summary>
    /// Performance metrics associated with a given node in the Rete network.
    /// </summary>
    public interface INodeMetrics
    {
        /// <summary>
        /// Id of the node with which these metrics are associated.
        /// </summary>
        int NodeId { get; }

        /// <summary>
        /// Number of elements stored in the node.
        /// If a node does not store elements, this value is <c>null</c>.
        /// </summary>
        int? ElementCount { get; }

        /// <summary>
        /// Cumulative number of elements that passed through this node during the propagation of inserted facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int InsertCount { get; }

        /// <summary>
        /// Cumulative number of elements that passed through this node during the propagation of updated facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int UpdateCount { get; }

        /// <summary>
        /// Cumulative number of elements that passed through this node during the propagation of retracted facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int RetractCount { get; }

        /// <summary>
        /// Cumulative number of milliseconds spent in this node handling the elements passed through it
        /// during the propagation of inserted facts.
        /// This time is counted since the last reset or since the creation of the session.
        /// This duration is exclusive of any time spent propagating facts through the downstream nodes.
        /// </summary>
        long InsertDurationMilliseconds { get; }

        /// <summary>
        /// Cumulative number of milliseconds spent in this node handling the elements passed through it
        /// during the propagation of updated facts.
        /// This time is counted since the last reset or since the creation of the session.
        /// This duration is exclusive of any time spent propagating facts through the downstream nodes.
        /// </summary>
        long UpdateDurationMilliseconds { get; }

        /// <summary>
        /// Cumulative number of milliseconds spent in this node handling the elements passed through it
        /// during the propagation of retracted facts.
        /// This time is counted since the last reset or since the creation of the session.
        /// This duration is exclusive of any time spent propagating facts through the downstream nodes.
        /// </summary>
        long RetractDurationMilliseconds { get; }

        /// <summary>
        /// Resets cumulative metrics associated with this node.
        /// </summary>
        void Reset();
    }

    internal class NodeMetrics : INodeMetrics
    {
        public int NodeId { get; }
        public int? ElementCount { get; set; }
        public int InsertCount { get; set; }
        public int UpdateCount { get; set; }
        public int RetractCount { get; set; }
        public long InsertDurationMilliseconds { get; set; }
        public long UpdateDurationMilliseconds { get; set; }
        public long RetractDurationMilliseconds { get; set; }

        public NodeMetrics(int nodeId)
        {
            NodeId = nodeId;
        }

        public void Reset()
        {
            InsertCount = 0;
            InsertDurationMilliseconds = 0;
            UpdateCount = 0;
            UpdateDurationMilliseconds = 0;
            RetractCount = 0;
            RetractDurationMilliseconds = 0;
        }
    }
}
