using System.Linq;

namespace NRules.Aggregators.Collections;

internal class FactGrouping<TKey, TElement> : FactCollection<TElement>, IGrouping<TKey, TElement>
{
    public FactGrouping(TKey key)
    {
        Key = key;
    }

    public TKey Key { get; set; }
}