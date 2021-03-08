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
        /// Cumulative number of elements that entered this node during the propagation of inserted facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int InsertInputCount { get; }

        /// <summary>
        /// Cumulative number of elements that entered this node during the propagation of updated facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int UpdateInputCount { get; }

        /// <summary>
        /// Cumulative number of elements that entered this node during the propagation of retracted facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int RetractInputCount { get; }

        /// <summary>
        /// Cumulative number of elements that exited this node during the propagation of inserted facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int InsertOutputCount { get; }

        /// <summary>
        /// Cumulative number of elements that exited this node during the propagation of updated facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int UpdateOutputCount { get; }

        /// <summary>
        /// Cumulative number of elements that exited this node during the propagation of retracted facts.
        /// This number is counted since the last reset or since the creation of the session.
        /// </summary>
        int RetractOutputCount { get; }

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
        public int InsertInputCount { get; set; }
        public int UpdateInputCount { get; set; }
        public int RetractInputCount { get; set; }
        public int InsertOutputCount { get; set; }
        public int UpdateOutputCount { get; set; }
        public int RetractOutputCount { get; set; }
        public long InsertDurationMilliseconds { get; set; }
        public long UpdateDurationMilliseconds { get; set; }
        public long RetractDurationMilliseconds { get; set; }

        public NodeMetrics(int nodeId)
        {
            NodeId = nodeId;
        }

        public void Reset()
        {
            InsertInputCount = 0;
            InsertOutputCount = 0;
            InsertDurationMilliseconds = 0;
            UpdateInputCount = 0;
            UpdateOutputCount = 0;
            UpdateDurationMilliseconds = 0;
            RetractInputCount = 0;
            RetractOutputCount = 0;
            RetractDurationMilliseconds = 0;
        }
    }
}
