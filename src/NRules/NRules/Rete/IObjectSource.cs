using System.Collections.Generic;

namespace NRules.Rete;

internal interface IObjectSource
{
    IReadOnlyCollection<Fact> GetFacts(IExecutionContext context);
    void Attach(IObjectSink sink);
}