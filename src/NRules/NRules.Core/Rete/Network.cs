namespace NRules.Core.Rete
{
    internal class Network : IObjectSink
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
    }
}