using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal abstract class AlphaNode : IObjectSink
    {
        protected AlphaNode()
        {
            ChildNodes = new List<AlphaNode>();
        }

        public IList<AlphaNode> ChildNodes { get; private set; }

        public abstract void PropagateAssert(Fact fact);
        public abstract void PropagateUpdate(Fact fact);
        public abstract void PropagateRetract(Fact fact);
        public abstract void ForceRetract(Fact fact);
    }
}