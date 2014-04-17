namespace NRules.Rete
{
    internal interface ITupleSink
    {
        void PropagateAssert(IExecutionContext context, Tuple tuple);
        void PropagateUpdate(IExecutionContext context, Tuple tuple);
        void PropagateRetract(IExecutionContext context, Tuple tuple);
    }
}