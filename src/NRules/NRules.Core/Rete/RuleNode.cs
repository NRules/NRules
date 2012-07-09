namespace NRules.Core.Rete
{
    internal class RuleNode : ITupleSink
    {
        private readonly string _ruleHandle;
        private readonly IEventSink _eventSink;

        public RuleNode(string ruleHandle, IEventSink eventSink)
        {
            _ruleHandle = ruleHandle;
            _eventSink = eventSink;
        }

        public void PropagateAssert(Tuple tuple)
        {
            var activation = new Activation(_ruleHandle, tuple);
            _eventSink.Activate(activation);
        }

        public void PropagateRetract(Fact fact)
        {
            //todo: deactivate
        }
    }
}