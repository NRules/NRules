namespace NRules.Core.Rete
{
    internal interface INetwork
    {
        IEventSource EventSource { get; }
        void PropagateAssert(Fact fact);
        void PropagateUpdate(Fact fact);
        void PropagateRetract(Fact fact);
    }

    internal class Network : IObjectSink, INetwork
    {
        private readonly RootNode _root;

        public IEventSource EventSource { get; private set; }

        public Network(RootNode root, IEventSource eventSource)
        {
            _root = root;
            EventSource = eventSource;
        }

        public void PropagateAssert(Fact fact)
        {
            _root.PropagateAssert(fact);
        }

        public void PropagateUpdate(Fact fact)
        {
            _root.PropagateUpdate(fact);
        }

        public void PropagateRetract(Fact fact)
        {
            _root.PropagateRetract(fact);
        }
    }
}