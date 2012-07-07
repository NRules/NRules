namespace NRules.Core.Rete
{
    internal interface IObjectSink
    {
        void PropagateAssert(Fact fact);
    }
}