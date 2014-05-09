using System;

namespace NRules.Diagnostics
{
    [Serializable]
    public class LinkInfo
    {
        public NodeInfo Source { get; private set; }
        public NodeInfo Target { get; private set; }

        internal LinkInfo(NodeInfo source, NodeInfo target)
        {
            Source = source;
            Target = target;
        }
    }
}