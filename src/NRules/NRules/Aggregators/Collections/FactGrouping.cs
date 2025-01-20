using System.Linq;

namespace NRules.Aggregators.Collections;

internal class FactGrouping<TKey, TElement>(TKey key) : FactCollection<TElement>, IGrouping<TKey, TElement>
{
    public TKey Key { get; set; } = key;
}