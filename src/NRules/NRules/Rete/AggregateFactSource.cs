using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Rete
{
    internal class AggregateFactSource : IFactSource
    {
        public AggregateFactSource(IEnumerable<IFact> facts)
        {
            Facts = facts;
        }

        public FactSourceType SourceType => FactSourceType.Aggregate;
        public IEnumerable<IFact> Facts { get; }
    }
}