using NRules.Diagnostics;

namespace NRules.Debug.Visualizer.Model
{
    public class ReteNode
    {
        private readonly NodeInfo _nodeInfo;

        public string NodeType { get { return _nodeInfo.NodeType.ToString(); } }
        public string Value { get { return string.Format("{0} {1}", _nodeInfo.NodeType, _nodeInfo.Details); } }
        public string[] Items { get { return _nodeInfo.Items; } }

        internal ReteNode(NodeInfo nodeInfo)
        {
            _nodeInfo = nodeInfo;
        }
    }
}