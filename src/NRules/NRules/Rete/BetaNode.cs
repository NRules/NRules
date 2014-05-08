namespace NRules.Rete
{
    internal abstract class BetaNode : ITupleSink, IObjectSink
    {
        public ITupleSource LeftSource { get; private set; }
        public IObjectSource RightSource { get; private set; }
        public ITupleSink Sink { get; private set; }

        protected BetaNode(ITupleSource leftSource, IObjectSource rightSource)
        {
            LeftSource = leftSource;
            RightSource = rightSource;

            LeftSource.Attach(this);
            RightSource.Attach(this);
        }

        public void Attach(ITupleSink sink)
        {
            Sink = sink;
        }

        public abstract void PropagateAssert(IExecutionContext context, Tuple tuple);
        public abstract void PropagateUpdate(IExecutionContext context, Tuple tuple);
        public abstract void PropagateRetract(IExecutionContext context, Tuple tuple);
        public abstract void PropagateAssert(IExecutionContext context, Fact fact);
        public abstract void PropagateUpdate(IExecutionContext context, Fact fact);
        public abstract void PropagateRetract(IExecutionContext context, Fact fact);

        public abstract void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor);
    }
}