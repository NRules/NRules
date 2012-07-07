namespace NRules.Core.Rete
{
    internal interface IObjectSource
    {
        void Attach(IObjectSink sink);
    }
}