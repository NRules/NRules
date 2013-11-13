namespace NRules.Rete
{
    internal abstract class BetaNode : ITupleSink, IObjectSink
    {
        public ITupleSource LeftSource { get; private set; }
        public IObjectSource RightSource { get; private set; }
        public ITupleSink Sink { get; set; }

        protected BetaNode(ITupleSource leftSource, IObjectSource rightSource)
        {
            LeftSource = leftSource;
            RightSource = rightSource;

            LeftSource.Attach(this);
            RightSource.Attach(this);
        }

        public abstract void PropagateAssert(IWorkingMemory workingMemory, Tuple tuple);
        public abstract void PropagateUpdate(IWorkingMemory workingMemory, Tuple tuple);
        public abstract void PropagateRetract(IWorkingMemory workingMemory, Tuple tuple);
        public abstract void PropagateAssert(IWorkingMemory workingMemory, Fact fact);
        public abstract void PropagateUpdate(IWorkingMemory workingMemory, Fact fact);
        public abstract void PropagateRetract(IWorkingMemory workingMemory, Fact fact);
    }
}