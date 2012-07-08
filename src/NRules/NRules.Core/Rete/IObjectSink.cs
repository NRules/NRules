namespace NRules.Core.Rete
{
    internal interface IObjectSink
    {
        void PropagateAssert(Fact fact);
        void PropagateUpdate(Fact fact);
        void PropagateRetract(Fact fact);
    }
}