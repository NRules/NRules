using NRules.Diagnostics;

namespace NRules.Debug.Visualizer.Model
{
    public class ReteNode
    {
        private readonly ReteNodeInfo _nodeInfo;

        public int Id { get { return _nodeInfo.Id; } }
        public string Value { get { return string.Format("{0} {1}", _nodeInfo.NodeType, _nodeInfo.Details); } }

        internal ReteNode(ReteNodeInfo nodeInfo)
        {
            _nodeInfo = nodeInfo;
        }
    }
}