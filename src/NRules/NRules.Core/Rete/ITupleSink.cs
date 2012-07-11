namespace NRules.Core.Rete
{
    internal interface ITupleSink
    {
        void PropagateAssert(Tuple tuple);
        void PropagateRetract(Tuple tuple);
    }
}