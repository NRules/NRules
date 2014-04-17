using System.Collections.Generic;

namespace NRules.Rete
{
    internal interface IObjectSource
    {
        IEnumerable<Fact> GetFacts(IExecutionContext context);
        void Attach(IObjectSink sink);
    }
}