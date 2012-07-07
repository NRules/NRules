using System.Collections.Generic;

namespace NRules.Core.Rete
{
    internal interface IObjectMemory : IObjectSource
    {
        IEnumerable<Fact> GetFacts();
    }
}