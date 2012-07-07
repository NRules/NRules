namespace NRules.Core.Rete
{
    internal class BetaAdapter : IObjectSink, ITupleSource
    {
        private ITupleSink _sink;

        public BetaAdapter(IObjectSource source)
        {
            source.Attach(this);
        }

        public void Attach(ITupleSink sink)
        {
            _sink = sink;
        }

        public void PropagateAssert(Fact fact)
        {
            var tuple = new Tuple(fact);
            _sink.PropagateAssert(tuple);
        }
    }
}